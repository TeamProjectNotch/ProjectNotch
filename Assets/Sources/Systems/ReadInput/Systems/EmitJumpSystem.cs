using System;
using Entitas;
using UnityEngine;

/// Emits Entities that represent players pressing "jump".
public class EmitJumpSystem : IExecuteSystem {

	readonly InputContext input;

	public EmitJumpSystem(Contexts contexts) {

		input = contexts.input;
	}

	public void Execute() {

		var jumping = Input.GetKeyDown(KeyCode.Space);
		if (!jumping) return;

		var e = input.CreateEntity();
		e.isJump = true;

		e.flagDestroy = true;
	}
}