using System.Collections.Generic;
using Entitas;

/// An INetworkableEntity bundled with its contextIndex. 
/// Exists to make it easier to retrieve a contextIndex when creating an 
/// EntityChange for the entity.
public struct ChangedEntityRecord {

	public INetworkableEntity entity;
	public int contextIndex;

	public ChangedEntityRecord(INetworkableEntity entity, int contextIndex) {

		this.entity = entity;
		this.contextIndex = contextIndex;
	}
}

/// Stores references to entities which need to be networked 
/// to the connection its entity represents.
[Networking]
public class EntitiesToSendComponent : IComponent {

    public HashSet<ChangedEntityRecord> records;
}
