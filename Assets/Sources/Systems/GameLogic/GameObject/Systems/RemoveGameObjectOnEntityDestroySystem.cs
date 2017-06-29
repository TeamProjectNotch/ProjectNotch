using Entitas;
using UnityEngine;
using System.Collections.Generic;

using Entitas.Unity;

/// Removes GameObjects of Entities
/// that are getting destroyed.
public class RemoveGameObjectOnEntityDestroySystem : ICleanupSystem {

	readonly IGroup<GameEntity> entities;

	public RemoveGameObjectOnEntityDestroySystem(Contexts contexts) {

		entities = contexts.game.GetGroup(
			GameMatcher.AllOf(GameMatcher.Destroy, GameMatcher.Destroy)
		);
	}

	public void Cleanup() {

		foreach (var e in entities.GetEntities()) {

			RemoveGameObject(e);
		}
	}

	void RemoveGameObject(GameEntity e) {

		var gameObject = e.gameObject.value;
		gameObject.Unlink();
		GameObject.Destroy(gameObject);

		e.RemoveGameObject();
	}
}