using System;

public class NetworkingSystems : MyFeature {

	public NetworkingSystems(Contexts contexts) : base("Networking systems") {

		AddServerSystems(contexts);
		AddClientSystems(contexts);

		Add(new SyncGameContextSystems(contexts));
		Add(new SyncInputContextSystems(contexts));
	}

	void AddServerSystems(Contexts contexts) {

		Add(new InitializeServerSystem(contexts));

		Add(new ServerReceiveSystem(contexts));
		Add(new HandleConnectingClientsSystem(contexts));
	}

	void AddClientSystems(Contexts contexts) {

		Add(new InitializeClientSystem(contexts));

		Add(new ClientReceiveSystem(contexts));
		Add(new HandleServerConnectionEstablishedSystem(contexts));
	}
}

public class SyncInputContextSystems : MyFeature {

	public SyncInputContextSystems(Contexts contexts) : base("SyncInputContext systems") {
		
		Add(new ComposeInputStateUpdateMessageSystem(contexts));
		Add(new HandleInputStateUpdateSystem(contexts));
	}
}