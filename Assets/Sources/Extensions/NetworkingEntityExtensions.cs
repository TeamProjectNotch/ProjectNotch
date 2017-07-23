using System;
using Entitas;

public static class NetworkingEntityExtensions {

	/// Enqueues a given message into the IncomingMessages component of the entity.
	/// A helper method that saves you the need to manually call ReplaceIcomingMessages.
	public static void EnqueueIncomingMessage(this NetworkingEntity e, INetworkMessage message) {

		if (!e.hasIncomingMessages) {
			throw new InvalidOperationException("Trying to enqueue an outgoing message to an entity which has no outgoing message queue!");
		}

		var queue = e.incomingMessages.queue;
		queue.Enqueue(message);
		e.ReplaceIncomingMessages(queue);
	}

	/// Enqueues a given message into the OutgoingMessages component of the entity.
	/// A helper method that saves you the need to manually call ReplaceOutgoingMessages.
	public static void EnqueueOutgoingMessage(this NetworkingEntity e, INetworkMessage message) {

		if (!e.hasOutgoingMessages) {
			
			throw new InvalidOperationException("Trying to enqueue an outgoing message to an entity which has no outgoing message queue!");
		}

		var queue = e.outgoingMessages.queue;
		queue.Enqueue(message);
		e.ReplaceOutgoingMessages(queue);
	}
}
