using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using Entitas;

/// Updates the player characters' input with values from the latest PlayerInputState from its input entity.
/// TEMP Only processes the last input.
[SystemAvailability(InstanceKind.All)]
public class ProcessMovementInputSystem : ProcessInputSystem {

	readonly GameContext game;

	public ProcessMovementInputSystem(Contexts contexts) : base(contexts) {

		game = contexts.game;
	}

	protected override void Process(GameEntity player, List<PlayerInputRecord> inputRecords) {

		if (!player.hasGameObject) return;

		var gameObject = player.gameObject.value;
		var characterBehaviour = gameObject.GetComponent<Character>();
		Assert.IsNotNull(characterBehaviour);

		gameObject.transform.SetState(player.transform.state);

		var numRecords = inputRecords.Count;
		if (numRecords == 0) {
			
			characterBehaviour.SimulateStep();
			player.ReplaceTransform(gameObject.transform.GetState());
			return;
		}

		ulong counter = 0;
		for (int i = 0; i < numRecords; ++i) {

			var inputRecord = inputRecords[i];
			var startTick = inputRecord.timestamp;
			var endTick = (i + 1 < inputRecords.Count) ? inputRecords[i + 1].timestamp : game.currentTick.value;
			var numTicksToSimulate = endTick - startTick + 1;

			characterBehaviour.SimulateStep(inputRecord.inputState);
			for (ulong tick = 1; tick < numTicksToSimulate; ++tick) {
				
				characterBehaviour.SimulateStep();
			}

			counter += numTicksToSimulate;
		}

		player.ReplaceTransform(gameObject.transform.GetState());

		//Debug.LogFormat("Simulated {0} ticks of character movement", counter);
	}
}
