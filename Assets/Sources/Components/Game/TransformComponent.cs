using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Entitas;
using fNbt;

public struct TransformState {

	public Vector3D position;
	public Quaternion rotation;
	public Vector3D scale;

	public TransformState(Vector3D position, Quaternion rotation, Vector3D scale) {

		this.position = position;
		this.rotation = rotation;
		this.scale = scale;
	}

	public TransformState(Vector3D position, Quaternion rotation) 
		: this(position, rotation, Vector3D.one) {}

	public TransformState(Vector3D position) 
		: this(position, Quaternion.identity, Vector3D.one) {}
}

/// The Entity has a transform, just like GameObjects. TransformState uses doubles instead of floats, however.
[Game]
public class TransformComponent : IComponent, IUnifiedSerializable {

	public TransformState state;

	public void Serialize<T>(T serializer) where T : IUnifiedSerializer {

		serializer.Serialize(ref state.position);
		serializer.Serialize(ref state.rotation);
		serializer.Serialize(ref state.scale);
	}
}
