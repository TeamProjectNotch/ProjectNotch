using System;
using Entitas;

public static class NetworkingEntityExtensions {

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
