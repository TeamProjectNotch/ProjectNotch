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
		var characterInput = gameObject.GetComponent<CharacterInput>();
		Assert.IsNotNull(characterInput);

		var inputState = inputRecords.Last().inputState;

		characterInput.moveAxes = inputState.moveAxes;
		characterInput.mouseMoveAxes = inputState.mouseMoveAxes;
		characterInput.isJump = inputState.buttonPressedJump;
	}

	// WIP! Frozen until I rework the system hierarchy.
	/*
	protected override void Process(GameEntity player, List<PlayerInputRecord> inputRecords) {

		if (!player.hasGameObject) return;

		var gameObject = player.gameObject.value;
		var transform = gameObject.transform;
		var rb = gameObject.GetComponent<Rigidbody>();

		//var totalTicksToSimulate = game.currentTick.value - inputRecords[0].timestamp + 1;
		//Debug.LogFormat("Simulating {0} ticks", totalTicksToSimulate);
		Debug.LogFormat("Simulating {0} input records", inputRecords.Count);

		var trState = player.transform.state;
		var rbState = player.rigidbodyState.state;

		for (int i = 0; i < inputRecords.Count; ++i) {

			var inputRecord = inputRecords[i];
			var startTick = inputRecord.timestamp;
			var endTick = (i + 1 < inputRecords.Count) ? inputRecords[i + 1].timestamp : game.currentTick.value;
			var numTicksToSimulate = endTick - startTick + 1;

			//Debug.LogFormat("Simulating {0} ticks", numTicksToSimulate);

			var speed = 8f;
			var surfaceNormal = transform.up; // TEMP
			var ticksToSeconds = Time.fixedDeltaTime;
			var secondsToTicks = 1f / ticksToSeconds;
			var numSecondsToSimulate = numTicksToSimulate * ticksToSeconds;
				
			// Stuff beyond this point goes into a separate method/methods.
			
			// Flat motion processing
			var moveAxes = inputRecord.inputState.moveAxes;
			var desiredDirection = transform.right * moveAxes.x + transform.forward * moveAxes.y;
			desiredDirection = Vector3.ProjectOnPlane(desiredDirection, surfaceNormal).normalized;

			// Not multiplied by `numTicksToSimulate` since movement on the plane is instantaneous, i.e. one input press per tick.
			trState.position += new Vector3D(desiredDirection * speed * (1f * ticksToSeconds));

			// Jump processing
			if (inputRecord.inputState.buttonPressedJump) {

				var jumpSpeed = 10f;
				var g = 9.81f;

				var heightChange = jumpSpeed * numSecondsToSimulate - g * numSecondsToSimulate * numSecondsToSimulate / 2f;
				heightChange = Math.Max(heightChange, 0f);
				trState.position += new Vector3D(transform.up * heightChange);

				var maxJumpTime = (2f * jumpSpeed / g);
				var verticalSpeedChange = numSecondsToSimulate <= maxJumpTime ? jumpSpeed - g * numSecondsToSimulate : 0f;
				var velocityChange = transform.up * verticalSpeedChange;

				rbState.velocity += new Vector3D(velocityChange);
			}
		}
			
		//rb.position = rb.position + motion;
		player.ReplaceTransform(trState);
		player.ReplaceRigidbodyState(rbState);
	}*/
}
