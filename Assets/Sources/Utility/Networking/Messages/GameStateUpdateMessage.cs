using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[NetworkMessage]
public class GameStateUpdateMessage : INetworkMessage {

	public EntityChange[] changes;

	public GameStateUpdateMessage() {}

	public GameStateUpdateMessage(EntityChange[] changes) {
		
		this.changes = changes;
	}

	public void Serialize<T>(T s) where T : IUnifiedSerializer {
		
        s.Serialize(ref changes);
    }
}
