using Entitas;

/// Stores the id of the Entity this Entity (a player) is currently holding.
[Game]
public class HandheldComponent : IComponent, IUnifiedSerializable {
	
    public ulong id;

	public void Serialize<T>(T s) where T : IUnifiedSerializer {

		s.Serialize(ref id);
	}
}