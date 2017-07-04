using System;
using Entitas;

/// Composes tick update messages. See TickUpdateMessage.
[SystemAvailability(InstanceKind.Server)]
public class ComposeTickUpdateSystem : IExecuteSystem {
	
	readonly NetworkingContext networking;
	readonly GameContext game;

	readonly IGroup<NetworkingEntity> clients;

	public ComposeTickUpdateSystem(Contexts contexts) {

		networking = contexts.networking;
		game = contexts.game;

		clients = networking.GetGroup(
			NetworkingMatcher.AllOf(NetworkingMatcher.Client, NetworkingMatcher.OutgoingMessages)
		);
	}

	public void Execute() {

		var message = MakeMessage();

		foreach (var e in clients.GetEntities()) {

			var queue = e.outgoingMessages.queue;
			queue.Enqueue(message);
			e.ReplaceOutgoingMessages(queue);
		}
	}

	INetworkMessage MakeMessage() {

		return new TickUpdateMessage(game.currentTick.value);
	}
}

