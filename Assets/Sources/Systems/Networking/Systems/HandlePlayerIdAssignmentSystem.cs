using System;
using Entitas;
using UnityEngine;

/// Client-side system.
/// Handles the PlayerIdAssingmentMessage coming from the server. Sets NetworkingContext.thisPlayerId.
public class HandlePlayerIdAssignmentSystem : ProcessMessageSystem<PlayerIdAssignmentMessage> {

	readonly GameContext game;

	public HandlePlayerIdAssignmentSystem(Contexts contexts) : base(contexts.networking) {

		game = contexts.game;
	}

	protected override IGroup<NetworkingEntity> GetMessageSources(IContext<NetworkingEntity> context) {

		return context.GetGroup(
			NetworkingMatcher.AllOf(NetworkingMatcher.Server, NetworkingMatcher.IncomingMessages)
		);
	}

	protected override void Process(PlayerIdAssignmentMessage message, NetworkingEntity source) {

		Debug.LogFormat("Received assigned player id: {0}", message.playerId);
		game.SetThisPlayerId(message.playerId);
	}
}
