﻿using System;
using Entitas;

/// Resets the character input manager on a player's entity so it's doesn't run on past inputs.
public class ResetPlayerInputSystem : IExecuteSystem {

	readonly IGroup<GameEntity> players;
	
	public ResetPlayerInputSystem(Contexts contexts) {

		players = contexts.game.GetGroup(
			GameMatcher.AllOf(GameMatcher.Player, GameMatcher.GameObject)
		);
	}

	public void Execute() {

		foreach (var e in players.GetEntities()) {

			var gameObject = e.gameObject.value;
			var inputManager = gameObject.GetComponent<CharacterInput>();
			inputManager.Reset();
		}
	}
}

