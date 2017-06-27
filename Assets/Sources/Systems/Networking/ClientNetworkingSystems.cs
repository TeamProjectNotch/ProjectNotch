using System;

/// Systems for the client-side networking stuff.
public class ClientNetworkingSystems : Feature {

	public ClientNetworkingSystems(Contexts contexts) : base("Networking (Client)") {

		Add(new InitializeClientSystem(contexts));

		Add(new ComposeInputMessageSystem(contexts));
		Add(new SendQueuedMessagesSystem(contexts));
	}
}
