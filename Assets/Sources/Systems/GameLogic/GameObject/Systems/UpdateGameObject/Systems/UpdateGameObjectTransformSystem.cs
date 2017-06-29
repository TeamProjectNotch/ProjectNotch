using System;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

/// Updates the Transform of a GameObject when its Entity's TransformComponent changes.
public class UpdateGameObjectTransformSystem : ReactiveSystem<GameEntity> {

	public UpdateGameObjectTransformSystem(Contexts contexts) : base(contexts.game) {}

	protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context) {

		return context.CreateCollector(
			GameMatcher
			.AllOf(GameMatcher.Transform, GameMatcher.GameObject)
			.Added()
		);
	}

	protected override bool Filter(GameEntity entity) {

		return true;
	}

	protected override void Execute(List<GameEntity> entities) {

		foreach (var e in entities) Process(e);
	}

	void Process(GameEntity e) {

		var transform = e.gameObject.value.transform;
		var state = e.transform.state;
		transform.SetState(state);

		//Debug.LogFormat("Updated Transform ({0}) with state ({1})", transform, state);
	}
}

