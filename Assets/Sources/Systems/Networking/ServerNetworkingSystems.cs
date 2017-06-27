using System;

/// Systems for the server-side networking stuff.
public class ServerNetworkingSystems : Feature {

	public ServerNetworkingSystems(Contexts contexts) : base("Networking (Server)") {

		Add(new InitializeServerSystem(contexts));

		Add(new ServerReceiveSystem(contexts));

		Add(new HandleConnectingClientsSystem(contexts));

		Add(new ServerSyncGameEntitiesSystems(contexts));
		Add(new HandleInputStateUpdateSystem(contexts));

		Add(new SendQueuedMessagesSystem(contexts));
	}
}
