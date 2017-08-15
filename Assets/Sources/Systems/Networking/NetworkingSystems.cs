using System;

/// Networking systems which Execute before the game logic, i.e. receiving state updates.
public class EarlyNetworkSystems : MyFeature {

	public EarlyNetworkSystems(Contexts contexts) : base("EarlyNetwork systems") {

        Add(new InitializeNetworkingSystem(contexts));

        Add(new ConnectToServerSystem(contexts)); // Client-only

		Add(new NetworkReceiveSystem(contexts));
		Add(new UpdateConnectionLatencySystem(contexts));

        // Handle Connecting Clients (Server-only)
		Add(new HandleConnectingClientsSystem(contexts));
        Add(new MarkAllEntitiesForNetworkSync(contexts));

		Add(new HandleServerConnectionEstablishedSystem(contexts)); // Client

		Add(new HandleTickUpdateSystem(contexts)); // Client
		Add(new HandleStateUpdateSystem(contexts));
	}
}

/// Networking systems which Execute after the game logic, i.e. sending state updates.
public class LateNetworkSystems : MyFeature {

	public LateNetworkSystems(Contexts contexts) : base("LateNetwork systems") {

		Add(new ComposeTickUpdateSystem(contexts)); // Server
		Add(new SendStateUpdatesSystems(contexts));

		Add(new SendQueuedMessagesSystem(contexts));
	}
}