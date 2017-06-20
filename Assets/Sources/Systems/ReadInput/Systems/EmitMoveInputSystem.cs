using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entitas;

/// Creates and cleans up Entities representing the move inputs of the player.
/// See MoveInputComponent.
public class EmitMoveInputSystem : IExecuteSystem {

	readonly InputContext input;

	public EmitMoveInputSystem(Contexts contexts) {

		input = contexts.input;
	}

	public void Execute() {

		var moveAxes = InputHelper.GetMoveAxes();
		if (moveAxes == Vector2.zero) return;

		var e = input.CreateEntity();
		e.AddMoveInput(moveAxes);

		e.flagDestroy = true;
	}
}

