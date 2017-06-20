using System;
using System.Collections.Generic;
using Entitas;

using UnityEngine;

/// When a GameObject of a computer monitor Entity is created, initializes the ScreenView therein to the correct resolution.
public class InitializeScreenViewSystem : ReactiveSystem<GameEntity> {

	public InitializeScreenViewSystem(Contexts contexts) : base(contexts.game) {}

	/// When a GameObjectComponent is added...
	protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context) {

		return context.CreateCollector(GameMatcher.GameObject.Added());
	}

	/// To an entity with a ScreenBufferComponent...
	protected override bool Filter(GameEntity entity) {

		return entity.hasScreenBuffer;
	}

	protected override void Execute(List<GameEntity> entities) {

		foreach (var e in entities) Process(e);
	}

	void Process(GameEntity e) {

		var screenView = e.gameObject.value.GetComponentInChildren<ScreenView>();
		if (screenView.isInitialized) {
			//Debug.LogError("GameObject added to a computer monitor entity with the ScreenView already initialized!");
			return;
		}

		screenView.resolution = e.screenBuffer.data.size;
		screenView.Initialize();
	}
}
