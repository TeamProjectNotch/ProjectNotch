using System;

/// Networking systems which Execute before the game logic, i.e. receiving state updates.
public class EarlyNetworkSystems : MyFeature {

	public EarlyNetworkSystems(Contexts contexts) : base("EarlyNetwork systems") {

		// TODO Merge these two systems
		Add(new InitializeServerSystem(contexts));
		Add(new InitializeClientSystem(contexts));

		// TODO Merge these two systems
		Add(new ServerReceiveSystem(contexts));
		Add(new ClientReceiveSystem(contexts));

		Add(new UpdateConnectionLatencySystem(contexts));

		Add(new HandleConnectingClientsSystem(contexts)); // Server
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