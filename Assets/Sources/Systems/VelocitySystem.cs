using System;
using Entitas;
using UnityEngine;

/// Moves entities not driven by Unity's physics based on their RigidbodyState
public class VelocitySystem : IExecuteSystem {

	readonly IGroup<GameEntity> entities;

	public VelocitySystem(Contexts contexts) {

		entities = contexts.game.GetGroup(GameMatcher
			.AllOf(GameMatcher.Transform, GameMatcher.RigidbodyState)
			.NoneOf(GameMatcher.GameObjectDriven)
		);
	}

	public void Execute() {

		foreach (var e in entities.GetEntities()) Process(e);
	}

	void Process(GameEntity e) {

		var transformState = e.transform.state;
		var rbState = e.rigidbodyState.state;

		var deltaTime = Time.fixedDeltaTime;
		transformState.position += rbState.velocity * deltaTime;
		transformState.rotation *= Quaternion.AngleAxis(
			angle:  (float)rbState.angularVelocity.magnitude * deltaTime,
			axis: (Vector3)rbState.angularVelocity.normalized
		);

		e.ReplaceTransform(transformState);
	}
}

