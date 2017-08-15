using System;
using Entitas;
using Entitas.CodeGeneration.Attributes;

[Game]
[Unique]
[NetworkSync(NetworkTargets.None)]
public class CurrentTickComponent : WrapperComponent<ulong>, IUnifiedSerializable {

	public void Serialize<T>(T s) where T : IUnifiedSerializer {

		s.Serialize(ref value);
	}
}
