using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[NetworkMessage]
public class GameStateUpdateMessage : INetworkMessage {

	public ulong timestamp;
	public EntityChange[] changes;

	public GameStateUpdateMessage() {}

	public GameStateUpdateMessage(ulong timestamp, EntityChange[] changes) {

		this.timestamp = timestamp;
		this.changes = changes;
	}

	public void Serialize<T>(T s) where T : IUnifiedSerializer {

		s.Serialize(ref timestamp);
        s.Serialize(ref changes);
    }
}
