using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entitas;

/// Creates InputEntities to signal presses of the fire button. Cleans them up afterwards.
public class EmitPressFireSystem : IExecuteSystem, ICleanupSystem {

	const KeyCode fireKeyCode = KeyCode.Mouse0;

	readonly InputContext input;

	InputEntity createdThisStep;

	public EmitPressFireSystem(Contexts contexts) {

		input = contexts.input;
	}

	public void Execute() {
		
		if (Input.GetKeyDown(fireKeyCode)) {

			var e = input.CreateEntity();
			e.isPressFire = true;

			createdThisStep = e;
		}
	}

	public void Cleanup() {

		if (createdThisStep != null) {
			
			createdThisStep.flagDestroy = true;
			createdThisStep = null;
		}
	}
}
