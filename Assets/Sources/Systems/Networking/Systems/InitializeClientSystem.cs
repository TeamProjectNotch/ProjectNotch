using System;
using Entitas;
using UnityEngine;
using UnityEngine.Networking;

[SystemAvailability(InstanceKind.Client)]
public class InitializeClientSystem : IInitializeSystem {

	const int maxNumConnections = 16;

	readonly NetworkingContext networking;

	public InitializeClientSystem(Contexts contexts) {

		networking = contexts.networking;
	}

	public void Initialize() {

		InitializeNetworkTransport();

		ConnectToServer();
	}

	void InitializeNetworkTransport() {

		NetworkTransport.Init();

		var topology = new HostTopology(
			ClientServerConnectionConfig.config, 
			maxNumConnections
		);
		var hostId = NetworkTransport.AddHost(topology);
		networking.SetHost(hostId);
	}

	void ConnectToServer() {

		var hostId = networking.host.id;

        byte errorCode;
		NetworkTransport.Connect(
            networking.host.id, "127.0.0.1", NetworkingHelper.serverPortNumber, 0, 
            out errorCode
        );
	}
}

