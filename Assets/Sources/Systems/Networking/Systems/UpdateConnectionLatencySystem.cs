using System;
using Entitas;
using UnityEngine;
using UnityEngine.Networking;

/// Updates the LatencyComponent of a connection entity with the appropriate values (half the network round-trip time)
public class UpdateConnectionLatencySystem : IExecuteSystem {

	readonly NetworkingContext networking;
	readonly IGroup<NetworkingEntity> connections;

	public UpdateConnectionLatencySystem(Contexts contexts) {

		networking = contexts.networking;
		connections = networking.GetGroup(NetworkingMatcher.Connection);
	}

	public void Execute() {

		foreach (var e in connections.GetEntities()) {

			byte errorCode;
			int rtt = NetworkTransport.GetCurrentRTT(networking.host.id, e.connection.id, out errorCode);
			if ((NetworkError)errorCode != NetworkError.Ok) {

				Debug.LogErrorFormat("Network error when trying to get current RTT: {0}", ((NetworkError)errorCode).ToString());
			}

			int currentMessageLatency = (rtt + 1) / 2; // Rounds up rtt / 2

			e.ReplaceLatency(currentMessageLatency);
		}
	}
}
