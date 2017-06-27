using System;
using Entitas;
using UnityEngine;

/// Client-side system.
/// Handles the HandleServerConnectionEstablishedMessage coming from the server.
/// Sets game state values like thisPlayerId and currentTick.
[SystemAvailability(InstanceKind.Client)]
public class HandleServerConnectionEstablishedSystem : ProcessMessageSystem<ServerConnectionEstablishedMessage> {

	readonly GameContext game;

	public HandleServerConnectionEstablishedSystem(Contexts contexts) : base(contexts.networking) {

		game = contexts.game;
	}

	protected override IGroup<NetworkingEntity> GetMessageSources(IContext<NetworkingEntity> context) {

		return context.GetGroup(
			NetworkingMatcher.AllOf(NetworkingMatcher.Server, NetworkingMatcher.IncomingMessages)
		);
	}

	protected override void Process(ServerConnectionEstablishedMessage message, NetworkingEntity source) {

		Debug.LogFormat("Set: player id: {0}, currentTick: {1}", message.playerId, message.currentTick);
		game.ReplaceThisPlayerId(message.playerId);
		game.ReplaceCurrentTick(message.currentTick);
	}
}
