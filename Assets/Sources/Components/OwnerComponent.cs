using System;
using Entitas;
using Entitas.CodeGeneration.Attributes;

/// The entity belongs to a player. Stores the id of that player.
[Game, Input, Networking]
public class OwnerComponent : IComponent, IUnifiedSerializable {

	[EntityIndex]
	public int value;

	public void Serialize<T>(T s) where T : IUnifiedSerializer {

		s.Serialize(ref value);
	}
}
