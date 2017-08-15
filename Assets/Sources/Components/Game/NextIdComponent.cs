using System;
using Entitas;
using Entitas.CodeGeneration.Attributes;

/// Stores the next unused unique id to assign to things.
[Game]
[Unique]
[NetworkSync(NetworkTargets.None)] // Automatic network sync disabled. Done manually.
public class NextIdComponent : WrapperComponent<ulong>, IUnifiedSerializable {

	public void Serialize<T>(T s) where T : IUnifiedSerializer {

		s.Serialize(ref value);
	}
}
