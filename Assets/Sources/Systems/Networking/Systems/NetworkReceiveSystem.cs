using System;
using System.Collections.Generic;
using Entitas;
using UnityEngine;
using UnityEngine.Networking;

/// Receives messages over the network and puts them in a queue (networking.incomingMessages).
/// Also registers connecting clients/servers.
[SystemAvailability(InstanceKind.Networked)]
public class NetworkReceiveSystem : IExecuteSystem {

	const int messageBufferSize = 1024 * 4;
	byte[] messageBuffer = new byte[messageBufferSize];

	readonly NetworkingContext networking;

	public NetworkReceiveSystem(Contexts contexts) {

		networking = contexts.networking;
	}

	public void Execute() {

		int connectionId, channelId;
		int receivedMessageSize;
		byte errorCode;

		while (true) {

			var eventType = NetworkTransport.ReceiveFromHost(
				networking.host.id, 
				out connectionId, out channelId, 
				messageBuffer, messageBufferSize, out receivedMessageSize, 
				out errorCode
			);

			var error = (NetworkError)errorCode;
			if (error != NetworkError.Ok) {

                Debug.LogError($"Error when receiving from network: {error}");
            }

			switch (eventType) {
				case NetworkEventType.Nothing: 
					return;
				case NetworkEventType.ConnectEvent:
					OnConnect(connectionId);
					break;
				case NetworkEventType.DisconnectEvent:
					OnDisonnect(connectionId);
					break;
				case NetworkEventType.DataEvent:
					OnReceive(connectionId, receivedMessageSize);
					break;
				default:
					break;
			}
		}
	}

	// Creates client connection Entities on connect.
	void OnConnect(int connectionId) {

		Debug.LogFormat("ConnectEvent: connectionId: {0}", connectionId);

		switch (ProgramInstance.thisInstanceKind) {
			case InstanceKind.Server:
				networking.CreateClientConnection(connectionId);
				break;
			case InstanceKind.Client:
				networking.CreateServerConnection(connectionId);
				break;
			default: 
				break;
		}
	}

	// Destroys client connection Entities on disconnect.
	void OnDisonnect(int connectionId) {

		Debug.LogFormat("DisconnectEvent: connectionId: {0}", connectionId);

		var e = networking.GetEntityWithConnection(connectionId);
		if (e == null) return;
		e.Destroy();
	}

	void OnReceive(int connectionId, int receivedMessageSize) {

		Debug.LogFormat("Received {0} bytes from connectionId {1}", receivedMessageSize, connectionId);

		// TODO Pool these message arrays to avoid tons of allocations and GC.
		var bytes = new byte[receivedMessageSize]; 
		Array.Copy(messageBuffer, bytes, receivedMessageSize);

	    var connectionEntity = networking.GetEntityWithConnection(connectionId);
		if (connectionEntity == null) return;

		var message = NetworkMessageSerializer.Deserialize(bytes);
		EnqueueIncomingMessage(message, connectionEntity);
	}

	void EnqueueIncomingMessage(INetworkMessage message, NetworkingEntity e) {

		var queue = e.hasIncomingMessages ? e.incomingMessages.queue : new Queue<INetworkMessage>();
		queue.Enqueue(message);
		e.ReplaceIncomingMessages(queue);
	}
}
