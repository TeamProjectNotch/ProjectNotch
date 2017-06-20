using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[NetworkMessage]
public class InputStateUpdateMessage : INetworkMessage {
	
	public EntityChange[] changes;

	public InputStateUpdateMessage() {}

	public InputStateUpdateMessage(EntityChange[] changes) {

		this.changes = changes;
	}

	public void Serialize<T>(T s) where T : IUnifiedSerializer {
		
        s.Serialize(ref changes);
	}
}

