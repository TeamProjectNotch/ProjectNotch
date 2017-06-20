using System;
using Entitas;
using UnityEngine;

/// Stores the direction in which the Entity (the player) is looking.
[Game]
public class LookDirectionComponent : WrapperComponent<Vector3>, IUnifiedSerializable {

	public void Serialize<T>(T s) where T : IUnifiedSerializer {

		s.Serialize(ref value);
	}
}
