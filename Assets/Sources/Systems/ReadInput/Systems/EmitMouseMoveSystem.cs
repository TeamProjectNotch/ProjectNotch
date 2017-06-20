using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entitas;

/// Creates and cleans up Entities representing the mouse move inputs of the player.
public class EmitMouseMoveSystem : IExecuteSystem {

	readonly InputContext input;

	public EmitMouseMoveSystem(Contexts contexts) {

		input = contexts.input;
	}

	public void Execute() {

		var mouseMoveAxes = InputHelper.GetMouseMoveAxes();
		if (mouseMoveAxes == Vector2.zero) return;

		var e = input.CreateEntity();
		e.AddMouseMoveInput(mouseMoveAxes);

		e.flagDestroy = true;
	}
}