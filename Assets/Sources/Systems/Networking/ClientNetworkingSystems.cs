using System;

/// Systems for the client-side networking stuff.
public class ClientNetworkingSystems : Feature {

	public ClientNetworkingSystems(Contexts contexts) : base("Networking (Client)") {

		Add(new InitializeClientSystem(contexts));

		Add(new ClientReceiveSystem(contexts));
		 
		Add(new HandlePlayerIdAssignmentSystem(contexts));
		Add(new ClientSyncGameEntitiesSystems(contexts));
		Add(new ComposeInputMessageSystem(contexts));

		Add(new SendQueuedMessagesSystem(contexts));
	}
}


