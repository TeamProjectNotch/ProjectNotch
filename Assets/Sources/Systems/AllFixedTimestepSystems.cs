using System;

/// A outline of the game loop:
/// Network receive
/// Read input (in update)
/// Game logic & process input & gameobject sync
/// Destroy entities marked for destruction
/// Increment tick.
public class AllFixedTimestepSystems : MyFeature {

	public AllFixedTimestepSystems(Contexts contexts) : base(contexts) {

		Add(new EnsureGameEntityIdSystem(contexts));
		Add(new EnsureInputEntityIdSystem(contexts));

		Add(new NetworkSystems(contexts));
		Add(new AllGameLogicSystems(contexts));
		Add(new SendQueuedMessagesSystem(contexts));

		Add(new DestroySystem(contexts));
		Add(new TicksSystem(contexts));
	}

	void AddNetworkReceive(Contexts contexts) {

		// Networking. Incoming data processing.
		Add(new ClientReceiveSystem(contexts));
		Add(new ClientSyncGameEntitiesSystems(contexts));
		Add(new HandleServerConnectionEstablishedSystem(contexts));
	}
}

public class NetworkSystems : MyFeature {

	public NetworkSystems(Contexts contexts) : base(contexts) {

		AddServerSystems(contexts);
		AddClientSystems(contexts);
	}

	void AddServerSystems(Contexts contexts) {
		
		Add(new InitializeServerSystem(contexts));

		Add(new ServerReceiveSystem(contexts));
		Add(new HandleConnectingClientsSystem(contexts));
		Add(new ServerSyncGameEntitiesSystems(contexts));
		Add(new HandleInputStateUpdateSystem(contexts));
	}

	void AddClientSystems(Contexts contexts) {

		Add(new InitializeClientSystem(contexts));

		Add(new ClientReceiveSystem(contexts));
		Add(new HandleGameStateUpdateSystem(contexts));
		Add(new HandleServerConnectionEstablishedSystem(contexts));

		Add(new ComposeInputMessageSystem(contexts));
	}
}
	
public class AllGameLogicSystems : MyFeature {

	public AllGameLogicSystems(Contexts contexts) : base(contexts) {

		// Initialize systems
		Add(new CreatePlayerWeaponSystem(contexts));
		Add(new TestCreateMonitorEntitySystem(contexts));
		// Initialize systems

		// Execute systems
		Add(new ProcessInputSystems(contexts));

		Add(new ProcessBulletCollisionSystem(contexts));
		Add(new DestroyWhenHealthZeroSystem(contexts));
		Add(new TestScreenBufferSystem(contexts));

		Add(new GameObjectSystems(contexts));
		// Execute systems

		// Cleanup systems
		Add(new CleanupCollisionSystem(contexts));
		// Cleanup systems
	}
}
