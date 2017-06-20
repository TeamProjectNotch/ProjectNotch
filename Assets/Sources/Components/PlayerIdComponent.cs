using System;
using Entitas;
using Entitas.CodeGeneration.Attributes;

/// Stores the id of the player this Entity represents or belongs to.
[Game, Networking]
public class PlayerIdComponent : IComponent, IUnifiedSerializable {

	[PrimaryEntityIndex]
	public int value;

	public void Serialize<T>(T s) where T : IUnifiedSerializer {

		s.Serialize(ref value);
	}
}
