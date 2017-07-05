using System;
using Entitas;
using UnityEngine;

/// Client-side system.
/// Handles the HandleServerConnectionEstablishedMessage coming from the server.
/// Sets game state values like thisPlayerId and currentTick.
[SystemAvailability(InstanceKind.Client)]
public class HandleServerConnectionEstablishedSystem : HandleMessageSystem<ServerConnectionEstablishedMessage> {

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

		game.ReplaceThisPlayerId(message.playerId);

		var messageDelay = source.hasLatency ? source.latency.ticks : 0;
		var messageDelayTicks = (ulong)Math.Round(messageDelay);
		game.ReplaceCurrentTick(message.currentTick + messageDelayTicks);

		Debug.LogFormat("Set: player id: {0}, currentTick: {1}", game.thisPlayerId, game.currentTick);
	}
}
