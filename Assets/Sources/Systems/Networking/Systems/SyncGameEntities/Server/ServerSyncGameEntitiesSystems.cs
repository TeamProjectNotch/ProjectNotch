using System;

/// Systems for sending the state of GameEntities to the clients over the network.
public class ServerSyncGameEntitiesSystems : Feature {

	public ServerSyncGameEntitiesSystems(Contexts contexts) : base("SyncGameEntities (Server)") {

		Add(new EnsureChangeFlagsSystem(contexts));
		Add(new UpdateChangeFlagsSystem(contexts));
		Add(new EnsureNetworkUpdatePrioritySystem(contexts));
		Add(new IncreaseNetworkUpdatePrioritySystem(contexts));

		Add(new ComposeGameChangeMessageSystem(contexts));
	}
}
