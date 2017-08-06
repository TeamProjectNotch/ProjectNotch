using Entitas;
using UnityEngine.Networking;

/// Sets ...
[SystemAvailability(InstanceKind.Networked)]
public class InitializeNetworkingSystem : IInitializeSystem {

    const int maxNumConnections = 16;

    readonly NetworkingContext networking;

    public InitializeNetworkingSystem(Contexts contexts) {

        networking = contexts.networking;
    }

    public void Initialize() {
            
        if (ProgramInstance.isServer) {

            networking.SetNextPlayerId(0);
        }

        InitializeNetworkTransport();
    }

    void InitializeNetworkTransport() {

        NetworkTransport.Init();

        int hostId = MakeHost();
        networking.SetHost(hostId);
    }

    int MakeHost() {

        var topology = new HostTopology(
            ClientServerConnectionConfig.config,
            maxNumConnections
        );

        if (ProgramInstance.isServer) {

            return NetworkTransport.AddHost(topology, NetworkingHelper.serverPortNumber);
        }

        if (ProgramInstance.isClient) {

            return NetworkTransport.AddHost(topology);
        }

        return -1;
    }
}