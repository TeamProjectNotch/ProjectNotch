using Entitas;
using System;

/// Indicates that a GameObject must be instantiated
/// to represent its entity.
/// Stores the path to a prefab.
[Game]
public class PrefabComponent : IComponent, IUnifiedSerializable {
	
	public string path = String.Empty;

	public void Serialize<T>(T s) where T : IUnifiedSerializer {

		s.Serialize(ref path);
	}
}
