using System;
using System.Collections.Generic;
using Entitas;

using UnityEngine;

/// Updates the GameObject of a screen through ScreenView when a screen buffer changes.
public class UpdateScreenViewSystem : ReactiveSystem<GameEntity> {
	
	public UpdateScreenViewSystem(Contexts contexts) : base(contexts.game) {}

	/// Will react whenever the ScreenBufferComponent of an entity gets added/replaced/updated.
	protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context) {
		
		return context.CreateCollector(GameMatcher.ScreenBuffer.Added());
	}

	/// Will only react to entities that have a GameObjectComponent.
	protected override bool Filter(GameEntity entity) {
		
		return entity.hasGameObject;
	}
		
	protected override void Execute(List<GameEntity> entities) {

		foreach (var e in entities) Process(e);
	}

	void Process(GameEntity e) {

		var screenView = e.gameObject.value.GetComponentInChildren<ScreenView>();
		Debug.Assert(screenView != null, screenView);

		var data = e.screenBuffer.data;
		screenView.SetColorIndices(data.colorIndicesPacked);
	}
}
