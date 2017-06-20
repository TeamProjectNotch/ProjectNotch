using System;
using System.Collections.Generic;
using Entitas;

/// Server-side system. 
/// Assigns ids to clients as connections with them are created. 
/// Sends a PlayerIdAssignmentMessage to each such client.
/// Creates a player GameEntity with that id.
public class AssignPlayerIdSystem : ReactiveSystem<NetworkingEntity> {

	readonly NetworkingContext networking;
	readonly GameContext game;

	public AssignPlayerIdSystem(Contexts contexts) : base(contexts.networking) {

		networking = contexts.networking;
		game = contexts.game;
	}

	protected override ICollector<NetworkingEntity> GetTrigger(IContext<NetworkingEntity> context) {
		
		return context.CreateCollector(NetworkingMatcher.Client.Added());
	}

	protected override bool Filter(NetworkingEntity entity) {return true;}

	protected override void Execute(List<NetworkingEntity> clients) {

		var nextPlayerId = networking.nextPlayerId.value;

		foreach (var e in clients) {

			var id = nextPlayerId++;

			e.AddPlayerId(id);
			e.EnqueueOutgoingMessage(new PlayerIdAssignmentMessage(id));

			CreatePlayerWith(id);
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

