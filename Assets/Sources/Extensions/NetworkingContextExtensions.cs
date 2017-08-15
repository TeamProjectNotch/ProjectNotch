using System;
using System.Collections.Generic;
using Entitas;

public static class NetworkingContextExtensions {

    public static NetworkingEntity CreateConnection(
        this NetworkingContext networking, int connectionId) {

        var e = networking.CreateEntity();
        e.AddConnection(connectionId);
        e.AddIncomingMessages(new Queue<INetworkMessage>());
        e.AddOutgoingMessages(new Queue<INetworkMessage>());

        return e;
    }

    public static NetworkingEntity CreateStateUpdateReceiverConnection(
        this NetworkingContext networking, int connectionId) {

        var e = networking.CreateConnection(connectionId);
        e.AddEntitiesToSend(new HashSet<ChangedEntityRecord>());
        e.AddDestroyedEntitiesBacklog(new HashSet<EntityChange>());

        return e;
    }

	/// Creates an Entity to represent a connection with a client.
	public static NetworkingEntity CreateClientConnection(this NetworkingContext networking, int connectionId) {

		var e = networking.CreateStateUpdateReceiverConnection(connectionId);
		e.isClient = true;

		return e;
	}

	/// Creates an Entity to represent a connection with a server.
	public static NetworkingEntity CreateServerConnection(this NetworkingContext networking, int connectionId) {

		var e = networking.CreateStateUpdateReceiverConnection(connectionId);
		e.isServer = true;

		return e;
	}
}
