using System;
using Entitas;

[NetworkMessage]
public class PlayerIdAssignmentMessage : INetworkMessage {

	public int playerId;

	public PlayerIdAssignmentMessage() {}

	public PlayerIdAssignmentMessage(int playerId) {

		this.playerId = playerId;
	}

	public void Serialize<T>(T s) where T : IUnifiedSerializer {

		s.Serialize(ref playerId);
	}
}
