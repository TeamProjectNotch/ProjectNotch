using System;
using UnityEngine;

public static class RigidbodyExtensions {

	public static RigidbodyState GetState(this Rigidbody rigidbody) {

		return new RigidbodyState(
			new Vector3D(rigidbody.velocity), 
			new Vector3D(rigidbody.angularVelocity)
		);
	}

	public static void SetState(this Rigidbody rigidbody, RigidbodyState state) {

		rigidbody.velocity = (Vector3)state.velocity;
		rigidbody.angularVelocity = (Vector3)state.angularVelocity;
	}
}
