﻿using System;
using System.Linq;
using System.Collections.Generic;
using Entitas;

/// Server-side system. 
/// Assigns ids to clients as connections with them are created. 
/// Sends a ServerConnectionEstablishedMessage to each such client.
/// Creates a player GameEntity with that id.
/// Marks all entities the new client needs to know about for sending over the network.
[SystemAvailability(InstanceKind.Server)]
public class HandleConnectingClientsSystem : ReactiveSystem<NetworkingEntity> {

    readonly Contexts contexts;
	readonly NetworkingContext networking;
	readonly GameContext game;

	public HandleConnectingClientsSystem(Contexts contexts) : base(contexts.networking) {

		networking = contexts.networking;
        game = contexts.game;
	}

	protected override ICollector<NetworkingEntity> GetTrigger(IContext<NetworkingEntity> context) {
		
		return context.CreateCollector(NetworkingMatcher.Client.Added());
	}

	protected override bool Filter(NetworkingEntity entity) {return true;}

	protected override void Execute(List<NetworkingEntity> newClients) {

		int nextPlayerId = networking.nextPlayerId.value;

		foreach (var client in newClients) {

			int playerId = nextPlayerId++;

			client.AddPlayer(playerId);
            client.EnqueueOutgoingMessage(new ServerConnectionEstablishedMessage() {
				playerId = playerId,
				currentTick = game.currentTick
			});

            CreatePlayerWith(playerId);
		}

		networking.ReplaceNextPlayerId(nextPlayerId);
	}

	void CreatePlayerWith(int playerId) {

		// TEMP Creates each new player at this hardcoded location.
		// Need some kind of spawn location set on the server.
		// Also need to keep track of players whose clients disconnected and then connected again.
		// So that the new player entity gets created where it left off, and not at the spawn.
		var position = new Vector3D(0, 1, -1);
		var transformState = new TransformState(position);
		game.CreatePlayer(playerId, transformState);
	}
}

