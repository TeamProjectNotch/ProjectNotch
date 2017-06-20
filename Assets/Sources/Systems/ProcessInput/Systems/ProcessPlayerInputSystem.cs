using System;
using UnityEngine;
using Entitas;

/// Updates the player characters' input with values from InputEntities.
/// Contains quite a lot of TEMP code as there can currently be just one player at a time.
/// 13.06.2017
public class ProcessPlayerInputSystem : IExecuteSystem {

	readonly IGroup<GameEntity> players;

	readonly IGroup<InputEntity> moveInputs;
	readonly IGroup<InputEntity> mouseInputs;
	readonly IGroup<InputEntity> jumps;

	public ProcessPlayerInputSystem(Contexts contexts) {

		players = contexts.game.GetGroup(
			GameMatcher.AllOf(GameMatcher.Player, GameMatcher.GameObject)
		);

		var input = contexts.input;
		moveInputs = input.GetGroup(InputMatcher.MoveInput);
		mouseInputs = input.GetGroup(InputMatcher.MouseMoveInput);
		jumps = input.GetGroup(InputMatcher.Jump);
	}

	public void Execute() {

		if (players.count <= 0) return;

		// TEMP This assumes that there's just one player. Should match inputs to players via a player id or something like that.
		var player = players.GetSingleEntity();
		var gameObject = player.gameObject.value;
		var inputManager = gameObject.GetComponent<CharacterInput>();

		// Apply move
		inputManager.moveAxes = Vector2.zero;
		foreach (var e in moveInputs.GetEntities()) {
			
			inputManager.moveAxes = e.moveInput.axes;
			e.flagDestroy = true;
		}

		// Apply mouse move
		inputManager.mouseMoveAxes = Vector2.zero;
		foreach (var e in mouseInputs.GetEntities()) {
			
			inputManager.mouseMoveAxes = e.mouseMoveInput.axes;
			e.flagDestroy = true;
		}

		// Apply jump
		inputManager.isJump = false;
		foreach (var e in jumps.GetEntities()) {
			
			inputManager.isJump = true;
			e.flagDestroy = true;
		}
	}
}
