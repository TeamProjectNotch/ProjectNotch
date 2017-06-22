using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Entitas;

/// Updates the player characters' input with values from the latest PlayerInputState from its input entity.
public class ProcessPlayerInputSystem : ReactiveSystem<InputEntity> {

	readonly GameContext game;
	readonly IGroup<GameEntity> players;

	readonly InputContext input;

	public ProcessPlayerInputSystem(Contexts contexts) : base(contexts.input) {

		game = contexts.game;
		players = game.GetGroup(
			GameMatcher.AllOf(GameMatcher.Player, GameMatcher.GameObject)
		);

		input = contexts.input;
	}

	protected override ICollector<InputEntity> GetTrigger(IContext<InputEntity> context) {

		return context.CreateCollector(InputMatcher.PlayerInputs.Added());
	}

	protected override bool Filter(InputEntity entity) {

		return entity.hasPlayer && entity.hasPlayerInputs;
	}

	protected override void Execute(List<InputEntity> entities) {

		foreach (var e in entities) Process(e);
	}

	void Process(InputEntity inputEntity) {

		var playerId = inputEntity.player.id;
		var gameEntity = game.GetEntityWithPlayer(playerId);
		if (!gameEntity.hasGameObject) return;

		var gameObject = gameEntity.gameObject.value;
		var inputManager = gameObject.GetComponent<CharacterInput>();

		PlayerInputState inputState;
		var didGetInput = GetMostRecentInput(inputEntity, out inputState);
		if (!didGetInput) {

			inputManager.Reset();
			return;
		}

		inputManager.moveAxes = inputState.moveAxes;
		inputManager.mouseMoveAxes = inputState.mouseMoveAxes;
		inputManager.isJump = inputState.buttonPressedJump;
	}

	bool GetMostRecentInput(InputEntity inputEntity, out PlayerInputState result) {

		result = new PlayerInputState();

		var inputs = inputEntity.playerInputs.inputs;
		if (inputs.Count <= 0) return false;

		var mostRecentRecord = inputs[inputs.Count - 1];
		var currentTick = game.currentTick.value;
		var timestamp = mostRecentRecord.timestamp;
		if (timestamp > currentTick) { 
			
			throw new Exception(String.Format(
				"The most recent input record is {0}, which is later than the current tick {1}", 
				timestamp, 
				currentTick
			));
		}

		result = mostRecentRecord.inputState;

		return true;
	}
}
