using System;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

/// Whenever RigidbodyStateComponent of an Entity changes, applies the new state to its rigidbody.
public class UpdateGameObjectRigidbodySystem : ReactiveSystem<GameEntity> {

	public UpdateGameObjectRigidbodySystem(Contexts contexts) : base(contexts.game) {}

	protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context) {

		return context.CreateCollector(
			GameMatcher
			.AllOf(GameMatcher.RigidbodyState, GameMatcher.GameObject)
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

		var rigidbody = e.gameObject.value.GetComponentInChildren<Rigidbody>();
		if (rigidbody == null) {
			Debug.LogError("RigidbodyState of an Entity changed, but its GameObject has no Rigidbody to apply the change to!");
			return;
		}

		var state = e.rigidbodyState.state;
		rigidbody.SetState(state);

		//Debug.LogFormat("Updated Rigidbody ({0}) with state ({1})", rigidbody, state);
	}
}
