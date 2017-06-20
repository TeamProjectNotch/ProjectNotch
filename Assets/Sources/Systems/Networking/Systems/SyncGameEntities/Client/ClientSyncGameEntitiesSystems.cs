using System;

/// Systems for applying the received state of GameEntities from the server.
public class ClientSyncGameEntitiesSystems : Feature {

	public ClientSyncGameEntitiesSystems(Contexts contexts) : base("SyncGameEntities (Client)") {

		Add(new HandleGameStateUpdateSystem(contexts));
	}
}
