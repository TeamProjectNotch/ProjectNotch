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

		networking.SetIds(
			hostId,
			ClientServerConnectionConfig.reliableFragmentedChannelId,
			ClientServerConnectionConfig.unreliableChannelId
		);
	}

	void ConnectToServer() {

		var hostId = networking.ids.host;

		byte errorCode;
		NetworkTransport.Connect(hostId, "127.0.0.1", NetworkingHelper.serverPortNumber, 0, out errorCode);
	}
}

