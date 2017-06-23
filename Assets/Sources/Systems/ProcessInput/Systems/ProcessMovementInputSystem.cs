using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using Entitas;

/// Updates the player characters' input with values from the latest PlayerInputState from its input entity.
/// TEMP only processes the last player input provided.
public class ProcessMovementInputSystem : ProcessInputSystem {

	public ProcessMovementInputSystem(Contexts contexts) : base(contexts) {}

	protected override void Process(GameEntity player, List<PlayerInputRecord> inputs) {

		if (!player.hasGameObject) return;

		var gameObject = player.gameObject.value;
		var inputManager = gameObject.GetComponent<CharacterInput>();
		Assert.IsNotNull(inputManager);

		var inputState = inputs[inputs.Count - 1].inputState;
		inputManager.moveAxes = inputState.moveAxes;
		inputManager.mouseMoveAxes = inputState.mouseMoveAxes;
		inputManager.isJump = inputState.buttonPressedJump;
	}
}
