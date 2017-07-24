using System;
using System.Collections.Generic;
using Entitas;
using UnityEngine;
using UnityEngine.Networking;

// TODO A lot of code here is the same as in ServerReceiveSystem. Should create a single ReceiveSystem which would create things with ConnectionComponent 
/// Receives messages over the network, deserializes them and puts them into a queue.
[SystemAvailability(InstanceKind.Client)]
public class ClientReceiveSystem : IExecuteSystem {

	const int messageBufferSize = 1024 * 4;
	byte[] messageBuffer = new byte[messageBufferSize];

	readonly NetworkingContext networking;

	public ClientReceiveSystem(Contexts contexts) {

		networking = contexts.networking;
	}

	public void Execute() {

		int connectionId, channelId;
		int receivedMessageSize;
		byte errorCode;

		while (true) {

			var eventType = NetworkTransport.ReceiveFromHost(
				networking.ids.host, 
				out connectionId, out channelId, 
				messageBuffer, messageBufferSize, out receivedMessageSize, 
				out errorCode
			);

			var error = (NetworkError)errorCode;
			if (error != NetworkError.Ok) Debug.LogError(error);

			switch (eventType) {
				
				case NetworkEventType.Nothing: 
					return;
				case NetworkEventType.ConnectEvent:
					OnConnect(connectionId);
					break;
				case NetworkEventType.DisconnectEvent:
					OnDisconnect(connectionId);
					break;
				case NetworkEventType.DataEvent:
					OnReceive(connectionId, receivedMessageSize);
					break;
				default:
					break;
			}
		}
	}
		
	void OnConnect(int connectionId) {

		Debug.LogFormat("ConnectEvent: connectionId: {0}", connectionId);

		networking.CreateServerConnection(connectionId);
	}
		
	void OnDisconnect(int connectionId) {

		Debug.LogFormat("DisconnectEvent: connectionId: {0}", connectionId);

		var e = networking.GetEntityWithConnection(connectionId);
		if (e == null) return;
		e.Destroy();
	}

	void OnReceive(int connectionId, int receivedMessageSize) {

		//Debug.LogFormat("Received {0} bytes from connectionId {1}", receivedMessageSize, connectionId);

		var connectionEntity = networking.GetEntityWithConnection(connectionId);
		if (connectionEntity == null) return;

		// TODO Pool these message arrays to avoid tons of allocations and GC.
		var bytes = new byte[receivedMessageSize]; 
		Array.Copy(messageBuffer, bytes, receivedMessageSize);

		var message = NetworkMessageSerializer.Deserialize(bytes);
		EnqueueIncomingMessage(message, connectionEntity);
	}

	void EnqueueIncomingMessage(INetworkMessage message, NetworkingEntity e) {

		var queue = e.hasIncomingMessages ? e.incomingMessages.queue : new Queue<INetworkMessage>();
		queue.Enqueue(message);
		e.ReplaceIncomingMessages(queue);
	}
}
