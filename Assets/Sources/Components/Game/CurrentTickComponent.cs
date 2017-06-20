using System;
using Entitas;
using Entitas.CodeGeneration.Attributes;

[Game]
[Unique]
public class CurrentTickComponent : IComponent, IUnifiedSerializable {
	
	public ulong value;

	public void Serialize<T>(T s) where T : IUnifiedSerializer {

		s.Serialize(ref value);
	}
}
