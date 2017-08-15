using Entitas;
using UnityEngine;
using UnityEngine.Networking;

/// Tries to connect to server via localhost.
[SystemAvailability(InstanceKind.Client)]
public class ConnectToServerSystem : IInitializeSystem {

    readonly NetworkingContext networking;

    public ConnectToServerSystem(Contexts contexts) {

        networking = contexts.networking;
    }

    public void Initialize() {

        ConnectToServer();
    }

    void ConnectToServer() {

        byte errorCode;
        NetworkTransport.Connect(
            networking.host.id, "127.0.0.1", NetworkingHelper.serverPortNumber, 0,
            out errorCode
        );

        var error = (NetworkError)errorCode;
        if (error != NetworkError.Ok) {
            
            Debug.LogError($"Error when connecting to server: {error}!");
        }
    }
}
