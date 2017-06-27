using System;

/// All the systems that are supposed to be run on the client during FixedUpdate.
public class ClientFixedTimestepSystems : Feature {

	public ClientFixedTimestepSystems(Contexts contexts) : base("FixedTimestep (Client)") {

		Add(new EnsureInputEntityIdSystem(contexts));

		Add(new TicksSystem(contexts));

		// Networking. Incoming data processing.
		Add(new ClientReceiveSystem(contexts));
		Add(new ClientSyncGameEntitiesSystems(contexts)); 
		Add(new HandleServerConnectionEstablishedSystem(contexts));

		Add(new ProcessInputSystems(contexts));
		Add(new VelocitySystem(contexts));

		Add(new GameObjectSystems(contexts));
		Add(new ClientNetworkingSystems(contexts));

		Add(new DestroySystem(contexts));
	}
}

