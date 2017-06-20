using System;
using Entitas;
using Entitas.Unity;
using UnityEngine;

/// Creates Entities from GameObjects in the scene and links each Entity to its GameObject.
public class CreateEntitiesFromGameObjectsSystem : IInitializeSystem {

	public readonly Contexts contexts;

	public CreateEntitiesFromGameObjectsSystem(Contexts contexts) {
		this.contexts = contexts;
	}

	public void Initialize() {

		var entityCreators = GameObject.FindObjectsOfType<EntityCreator>();

		foreach (var entityCreator in entityCreators) {

			var hasEntity = entityCreator.gameObject.GetEntityLink() != null;
			if (hasEntity) continue;

			var e = entityCreator.CreateEntity(contexts);
			e.AddGameObject(entityCreator.gameObject);
		}
	}
}

