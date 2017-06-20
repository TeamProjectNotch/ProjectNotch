using System;
using Entitas;
using UnityEngine;

[Serializable]
public struct RigidbodyState {

	public static readonly RigidbodyState rest = 
		new RigidbodyState(Vector3D.zero, Vector3D.zero);
	
	public Vector3D velocity;
	public Vector3D angularVelocity;

	public bool isAtRest {
		get {
			
			return 
				velocity == Vector3D.zero &&
				angularVelocity == Vector3D.zero;
		}
	}

	public RigidbodyState(Vector3D velocity, Vector3D angularVelocity) {

		this.velocity = velocity;
		this.angularVelocity = angularVelocity;
	}

	public RigidbodyState(Vector3D velocity) {

		this.velocity = velocity;
		this.angularVelocity = Vector3D.zero;
	}
}

/// Stores the physics-related data from the GameObject.
[Game]
public class RigidbodyStateComponent : IComponent, IUnifiedSerializable {

	public RigidbodyState state;

	public void Serialize<T>(T s) where T : IUnifiedSerializer {

		bool atRest = s.isWriting ? state.isAtRest : false;
		s.Serialize(ref atRest);

		if (!atRest) {
			
			s.Serialize(ref state.velocity);
			s.Serialize(ref state.angularVelocity);
		}
	}
}
