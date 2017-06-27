using System;
using Entitas;

/// This message is sent from the server to a newly connected client.
[NetworkMessage]
public class ServerConnectionEstablishedMessage : INetworkMessage {

	public int playerId;
	public ulong currentTick;

	public ServerConnectionEstablishedMessage() {}

	public void Serialize<T>(T s) where T : IUnifiedSerializer {

		s.Serialize(ref playerId);
		s.Serialize(ref currentTick);
	}
}
