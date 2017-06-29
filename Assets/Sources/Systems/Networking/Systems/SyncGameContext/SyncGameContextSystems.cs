using System;

/// Systems for sending the state of GameEntities to the clients over the network.
public class SyncGameContextSystems : MyFeature {

	public SyncGameContextSystems(Contexts contexts) : base("SyncGameEntities systems") {

		// Server
		Add(new EnsureChangeFlagsSystem(contexts));
		Add(new UpdateChangeFlagsSystem(contexts));
		Add(new EnsureNetworkUpdatePrioritySystem(contexts));
		Add(new IncreaseNetworkUpdatePrioritySystem(contexts));

		Add(new ComposeGameStateUpdateMessageSystem(contexts));

		// Client
		Add(new HandleGameStateUpdateSystem(contexts));
	}
}
