using System;
using Entitas;
using Entitas.CodeGeneration.Attributes;

/// Stores the id of an Entity.
/// Can be used for quick Entity access like "contexts.game.GetEntityWithId(id)" 
[Game, Input]
public class IdComponent : IComponent, IUnifiedSerializable {

	[PrimaryEntityIndex]
	public ulong value;

	public void Serialize<T>(T s) where T : IUnifiedSerializer {

		s.Serialize(ref value);
	}
}
