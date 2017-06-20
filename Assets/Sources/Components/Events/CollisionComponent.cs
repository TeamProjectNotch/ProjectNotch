using System;
using Entitas;

/// Indicates that there's been a collision between two Entities. 
/// Stores IDs of those Entities.
/// There MAY be two collision Entities per collision ('a' and 'b') 
/// - 'a' colliding with 'b' and 'b' with 'a'.
[Events]
public class CollisionComponent : IComponent {
	
	public ulong selfEntityId;
	public ulong otherEntityId;
}
