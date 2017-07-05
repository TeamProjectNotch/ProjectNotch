using System;
using System.Collections.Generic;
using Entitas;
using UnityEngine;
using UnityEngine.Networking;

/// Cycles through all connections with an outgoing message queue. 
/// Sends all messages from each queue every simstep.
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
		var bytes = NetworkMessageSerializer.Serialize(queue);
		Send(bytes, e.connection.id);

		queue.Clear();
		e.ReplaceOutgoingMessages(queue);
	}

	void Send(byte[] bytes, int connectionId) {

		var channelId = ClientServerConnectionConfig.unreliableFragmentedChannelId;

		byte errorCode;
		NetworkTransport.Send(ids.host, connectionId, channelId, bytes, bytes.Length, out errorCode);

		var error = (NetworkError)errorCode;
		if (error != NetworkError.Ok) {
			Debug.LogErrorFormat("Error while sending a message: {0}", error.ToString());
		}
	}
}
