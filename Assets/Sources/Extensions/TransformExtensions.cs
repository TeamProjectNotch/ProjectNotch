using System;
using Entitas;
using UnityEngine;

public static class TransformExtensions {

	public static TransformState GetState(this Transform transform) {
		
		return new TransformState(
			new Vector3D(transform.position),
			transform.rotation,
			new Vector3D(transform.localScale)
		);
	}

	/// Applies some TransformState of an entity to a UnityEngine.Transform.
	/// originPosition is the position of the origin of *the scene* in the big world.
	/// Use it to make sure the GameObject of an Entity has the same transform.
	public static void SetState(this Transform transform, TransformState state, Vector3D originPosition) {

		transform.position = (Vector3)(state.position - originPosition);
		transform.rotation = state.rotation;
		transform.localScale = (Vector3)state.scale;
	}

	public static void SetState(this Transform transform, TransformState state) {

		transform.SetState(state, originPosition: Vector3D.zero);
	}

	/// Just like Transform.Find, but *can* find children of children and so on.
	public static Transform FindRecursive(this Transform parent, string name) {
		
		var result = parent.Find(name);
		if (result) return result;

		foreach (Transform child in parent) {
			
			result = child.FindRecursive(name);
			if (result) return result;
		}

		return null;
	}
}
