using System;
using Entitas;
using UnityEngine;
using UnityEngine.Networking;

/// Sets the context-global IdsComponent. The host id and channel ids.
public class InitializeServerSystem : IInitializeSystem {
	
	const int maxNumConnections = 16;

	readonly NetworkingContext networking;

	public InitializeServerSystem(Contexts contexts) {

		networking = contexts.networking;
	}

	public void Initialize() {

		networking.SetNextPlayerId(0);

		InitializeNetworkTransport();
	}

	void InitializeNetworkTransport() {

		NetworkTransport.Init();

		var topology = new HostTopology(
			ClientServerConnectionConfig.config, 
			maxNumConnections
		);
		var hostId = NetworkTransport.AddHost(topology, NetworkingHelper.serverPortNumber);

		networking.SetIds(
			hostId,
			ClientServerConnectionConfig.reliableFragmentedChannelId,
			ClientServerConnectionConfig.unreliableChannelId
		);
	}
}

