using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Creates a simple entity. Adds RigidbodyState if finds a non-kinematic Rigidbody to work with.
public class EntityCreatorSimple : EntityCreator {

	[SerializeField]
	string prefabPath = "None";

	public override GameEntity CreateEntity(Contexts contexts) {

		var e = contexts.game.CreateEntity();

		e.AddTransform(transform.GetState());
		TryAddRigidbodyState(e);
		e.isGameObjectDriven = true;

		e.AddPrefab(prefabPath);

		return e;
	}

	void TryAddRigidbodyState(GameEntity e) {
		
		var rigidbody = GetComponentInChildren<Rigidbody>();
		if (rigidbody != null && !rigidbody.isKinematic) {
			e.AddRigidbodyState(rigidbody.GetState());
		}
	}
}
