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
		var mover = gameObject.GetComponent<CharacterMover>();
		Assert.IsNotNull(mover);

		gameObject.transform.SetState(player.transform.state);
		mover.velocity = (Vector3)player.rigidbodyState.state.velocity;

		var numRecords = inputRecords.Count;
		if (numRecords == 0) {
			
			characterBehaviour.SimulateStep();
			SyncPosAndVelocity(player, gameObject.transform.GetState(), mover.velocity);
			return;
		}

		ulong counter = 0;
		for (int i = 0; i < numRecords; ++i) {

			var inputRecord = inputRecords[i];
			var startTick = inputRecord.timestamp;
			var endTick = (i + 1 < inputRecords.Count) ? inputRecords[i + 1].timestamp : game.currentTick.value;

			characterBehaviour.SimulateStep(inputRecord.inputState);
			counter++;
			for (ulong tick = startTick + 1; tick < endTick; ++tick) {
				
				characterBehaviour.SimulateStep();
				counter++;
			}
		}

		//Debug.LogFormat("Simulated {0} ticks of character movement", counter);

		SyncPosAndVelocity(player, gameObject.transform.GetState(), mover.velocity);
	}

	void SyncPosAndVelocity(GameEntity player, TransformState trState, Vector3 velocity) {

		player.ReplaceTransform(trState);

		var rbState = player.rigidbodyState.state;
		rbState.velocity = new Vector3D(velocity);
		player.ReplaceRigidbodyState(rbState);
	}
}
