using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Entitas;
using UnityEngine;

/// Applies changes received over the network. 
/// Gets them from an incoming message queue on the Entity that represents a connection with the server.
public class HandleGameStateUpdateSystem : ProcessMessageSystem<GameStateUpdateMessage> {

	readonly GameContext game;

	public HandleGameStateUpdateSystem(Contexts contexts) : base(contexts.networking) {

		game = contexts.game;
	}

	protected override IGroup<NetworkingEntity> GetMessageSources(IContext<NetworkingEntity> context) {
		
		return context.GetGroup(
			NetworkingMatcher.AllOf(NetworkingMatcher.Server, NetworkingMatcher.IncomingMessages)
		);
	}

	protected override void Process(GameStateUpdateMessage message, NetworkingEntity source) {
		
		Debug.LogFormat("Received game state update from tick {0}, current tick {1}", 
			message.timestamp, 
			game.hasCurrentTick ? game.currentTick.value.ToString() : (-1).ToString()
		);

		message.changes.Each(Apply);
		Debug.LogFormat("Changes applied this step: {0}", message.changes.Length);
	}

	void Apply(EntityChange change) {

		var e = game.GetEntityWithId(change.entityId);
		if (e == null) {

			if (change.isRemoval) {
				
				UnityEngine.Debug.Log("Can't apply an EntityChange, since it's Entity doesn't exist.");
				return;
			}

			Debug.LogFormat("Entity with id {0} not found. Creating...", change.entityId);
			e = game.CreateEntity();
		}

		change.Apply(e);
	}
}
