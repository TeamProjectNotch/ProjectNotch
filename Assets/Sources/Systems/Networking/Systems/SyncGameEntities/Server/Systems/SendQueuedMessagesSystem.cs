using System;
using System.Collections.Generic;
using Entitas;
using UnityEngine;
using UnityEngine.Networking;

/// Cycles through all connections with an outgoing message queue. 
/// Sends one message from each queue every simstep.
[SystemAvailability(InstanceKind.Server | InstanceKind.Client)]
public class SendQueuedMessagesSystem : IExecuteSystem {

	readonly NetworkingContext networking;
	readonly IGroup<NetworkingEntity> entities;

	IdsComponent ids;

	public SendQueuedMessagesSystem(Contexts contexts) {

		networking = contexts.networking;
		entities = networking.GetGroup(
			NetworkingMatcher.AllOf(NetworkingMatcher.OutgoingMessages, NetworkingMatcher.Connection)
		);
	}

	public void Execute() {

		ids = networking.ids;

		foreach (var e in entities.GetEntities()) Process(e);
	}

	void Process(NetworkingEntity e) {

		var queue = e.outgoingMessages.queue;
		if (queue.Count <= 0) return;
		var message = queue.Dequeue();
		e.ReplaceOutgoingMessages(queue);

		byte[] bytes = NetworkMessageSerializer.Serialize(message);
		Send(bytes, e.connection.id);
	}

	void Send(byte[] bytes, int connectionId) {

		byte errorCode;
		NetworkTransport.Send(ids.host, connectionId, ids.channelReliableFragmented, bytes, bytes.Length, out errorCode);

		var error = (NetworkError)errorCode;
		if (error != NetworkError.Ok) {
			Debug.LogErrorFormat("Error while sending a message: {0}", error.ToString());
		}
	}
}
