using System;
using Entitas;

using UnityEngine;

/// Tries to create a monitor entity.
[SystemAvailability(InstanceKind.ServerAndSingleplayer)]
public class TestCreateMonitorEntitySystem : IInitializeSystem {

	readonly GameContext game;

	public TestCreateMonitorEntitySystem(Contexts contexts) {

		game = contexts.game;
	}

	public void Initialize() {

		var transformState = GetTransformState();
        var screenState = GetScreenBufferState();

		game.CreateMonitor(transformState, screenState);
	}

    TransformState GetTransformState() {

		return new TransformState(
			new Vector3D(-10, 2, 0),
			Quaternion.Euler(0f, -30f, 0f),
			Vector3D.one
		);
	}

	ScreenBufferState GetScreenBufferState() {

		return new ScreenBufferState(new Vector2Int(64, 48));
	}
}

