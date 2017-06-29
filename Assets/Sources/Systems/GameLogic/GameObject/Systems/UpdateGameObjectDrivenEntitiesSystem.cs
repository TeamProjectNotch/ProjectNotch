using System;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

/// For Entities marked as GameObjectDriven, updates their state based on their GameObjects.
[SystemAvailability(InstanceKind.All)]
public class UpdateGameObjectDrivenEntitiesSystem : IExecuteSystem {

	readonly IGroup<GameEntity> drivenByGameObjects;

	public UpdateGameObjectDrivenEntitiesSystem(Contexts contexts) {

		drivenByGameObjects = contexts.game.GetGroup(
			GameMatcher.AllOf(GameMatcher.GameObjectDriven, GameMatcher.GameObject)
		);
	}

	public void Execute() {

		foreach (var e in drivenByGameObjects.GetEntities()) Process(e);
	}

	void Process(GameEntity e) {

		var go = e.gameObject.value;

		e.ReplaceTransform(go.transform.GetState());

		if (e.hasRigidbodyState) {
			
			var rigidbody = go.GetComponentInChildren<Rigidbody>();
			if (rigidbody) {
				
				e.ReplaceRigidbodyState(rigidbody.GetState());
			}
		}
	}
}
