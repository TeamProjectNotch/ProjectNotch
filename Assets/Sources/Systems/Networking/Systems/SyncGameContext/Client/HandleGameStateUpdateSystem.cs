using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Entitas;
using UnityEngine;

// TODO Make an abstract ProcessStateUpdateMessageSystem with virtual methods like OnWillApplyEntityChange, OnDidApplyEntityChange.
/// Applies game changes received over the network. 
/// Gets them from an incoming message queue on the Entity that represents a connection with the server.
[SystemAvailability(InstanceKind.Client)]
public class HandleGameStateUpdateSystem : HandleMessageSystem<GameStateUpdateMessage> {

	readonly GameContext game;
	readonly InputContext input;

	public HandleGameStateUpdateSystem(Contexts contexts) : base(contexts.networking) {

		game = contexts.game;
		input = contexts.input;
	}

	protected override IGroup<NetworkingEntity> GetMessageSources(IContext<NetworkingEntity> context) {
		
		return context.GetGroup(
			NetworkingMatcher.AllOf(NetworkingMatcher.Server, NetworkingMatcher.IncomingMessages)
		);
	}

	protected override void Process(GameStateUpdateMessage message, NetworkingEntity source) {

		/*
		Debug.LogFormat("Received game state update from tick {0}, current tick {1}", 
			message.timestamp, 
			game.hasCurrentTick ? game.currentTick.value.ToString() : (-1).ToString()
		);*/
			
		foreach (var change in message.changes) {
			
			Process(source, message, change);
		}

		//Debug.LogFormat("Changes applied this step: {0}", message.changes.Length);
	}

	void Process(NetworkingEntity source, GameStateUpdateMessage message, EntityChange change) {

		var e = game.GetEntityWithId(change.entityId);
		if (e == null) {

			if (change.isRemoval) {
				
				UnityEngine.Debug.Log("Can't apply an EntityChange, since it's Entity doesn't exist.");
				return;
			}

			//Debug.LogFormat("Entity with id {0} not found. Creating...", change.entityId);
			e = game.CreateEntity();
		}

		change.Apply(e);
		OnDidApply(source, message, change, e);
	}

	// TODO Make this a protected virtual method inheritors would override.
	/// <summary>
	/// When this client's player entity is updated over the network, 
	/// changes the ProcessInputsComponent on that player's input entity so that
	/// all inputs, starting with the tick the message was sent, will be reprocessed.
	/// </summary>
	/// <param name="source">The connection entity the message came from.</param>
	/// <param name="message">The message the applied entity change was part of.</param>
	/// <param name="change">The entity change that was applied.</param>
	/// <param name="e">The game entity the change was applied to.</param>
	void OnDidApply(NetworkingEntity source, GameStateUpdateMessage message, EntityChange change, GameEntity e) {

		if (!e.hasPlayer) return;
		if (!game.hasThisPlayerId) return;

		var playerId = e.player.id;
		if (playerId == game.thisPlayerId.value) {

			var playerInputEntity = input.GetEntityWithPlayer(playerId);
			if (playerInputEntity == null) return;

			var messageDelay = source.hasLatency ? (ulong)Math.Ceiling(source.latency.ticks) : 0;
			// It's not just message.timestamp because at message.timestamp the
			// server only had received inputs from message.timestamp - messageDelay.
			var inputProcessTick = message.timestamp - messageDelay + 1;
			if (playerInputEntity.hasProcessInputs) {

				inputProcessTick = Math.Min(inputProcessTick, playerInputEntity.processInputs.startTick);
			}
			playerInputEntity.ReplaceProcessInputs(inputProcessTick);
			//Debug.LogFormat("Will reprocess inputs since tick {0}, now is {1}, msg delay is {2}", inputProcessTick, game.currentTick.value, messageDelay);

			// Delete input records earlier than message timestamp.
			var inputs = playerInputEntity.playerInputs.inputRecords;
			inputs.RemoveAll(record => record.timestamp < inputProcessTick);
			playerInputEntity.ReplacePlayerInputs(inputs);
		}
	}
}
