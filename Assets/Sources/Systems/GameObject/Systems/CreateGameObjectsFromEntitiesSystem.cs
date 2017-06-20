using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Entitas;
using Entitas.CodeGeneration.Attributes;

/// Creates GameObjects to represent Entities when needed. For now just creates GOs for all Entities with a PrefabComponent.
public class CreateGameObjectsFromEntitiesSystem : ReactiveSystem<GameEntity> {

    public CreateGameObjectsFromEntitiesSystem(Contexts contexts) : base(contexts.game) {}

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context) {
		
        return context.CreateCollector(GameMatcher.Prefab.Added());
    }

    protected override bool Filter(GameEntity entity) {
        
		return !entity.hasGameObject;
    }

	protected override void Execute(List<GameEntity> entities) {

        foreach (var e in entities) CreateGameObjectFor(e);
	}

	void CreateGameObjectFor(GameEntity e) {

		var prefab = GetPrefab(e.prefab.path);
		var gameObject = Instantiate(prefab);

		e.AddGameObject(gameObject);
	}

	GameObject GetPrefab(String path) {
		
		var prefab = Resources.Load<GameObject>(path);
		if (prefab == null) {
			throw new NullReferenceException(String.Format("Can't find prefab at path {0}!", path));	
		}
		return prefab;
	}

	GameObject Instantiate(GameObject prefab) {
		
		var gameObject = GameObject.Instantiate(prefab, GameObjects.root);
		return gameObject;
	}
}
