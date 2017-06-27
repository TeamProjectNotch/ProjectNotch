using System;
using Entitas;

using UnityEngine;

/// Tries to create a monitor entity.
[SystemAvailability(InstanceKind.Server | InstanceKind.Singleplayer)]
public class TestCreateMonitorEntitySystem : IInitializeSystem {

	readonly GameContext game;

	public TestCreateMonitorEntitySystem(Contexts contexts) {

		game = contexts.game;
	}

	public void Initialize() {

		var transformState = GettransformState();
		var screenData = GetScreenBufferState();

		game.CreateMonitor(transformState, screenData);
	}

	TransformState GettransformState() {

		var data = new TransformState(
			new Vector3D(-10, 2, 0),
			Quaternion.Euler(0f, -30f, 0f),
			Vector3D.one
		);

		return data;
	}

	ScreenBufferState GetScreenBufferState() {

		var data = new ScreenBufferState(new Vector2Int(64, 48));
		return data;
	}
}

