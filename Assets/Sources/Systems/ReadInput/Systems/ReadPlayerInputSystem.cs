using System;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

/// Client-side system.
/// Reads values from UnityEngine.Input and adds PlayerInputRecord|s to the player's input entity
[SystemAvailability(InstanceKind.CleintAndSingleplayer)]
public class ReadPlayerInputSystem : IExecuteSystem {

	readonly GameContext game;
	readonly InputContext input;

	public ReadPlayerInputSystem(Contexts contexts) {

		game = contexts.game;
		input = contexts.input;
	}

	public void Execute() {

		if (!game.hasThisPlayerId) return;
		if (!game.hasCurrentTick)  return;

		var inputEntity = GetInputEntityBy(game.thisPlayerId.value);
		ReadInputInto(inputEntity);
	}

	InputEntity GetInputEntityBy(int playerId) {

		return 
			input.GetEntityWithPlayer(playerId) ?? 
			input.CreatePlayerInputEntity(playerId);
	}

	void ReadInputInto(InputEntity playerInputEntity) {

		var inputState = new PlayerInputState();
		var didReadAny = ReadPlayerInputStateInto(ref inputState); 
		if (!didReadAny) return;

		var currentTick = game.currentTick.value;
		var newRecord = new PlayerInputRecord(currentTick, inputState);

		InsertNewInputRecord(playerInputEntity, newRecord);
	}

	bool ReadPlayerInputStateInto(ref PlayerInputState result) {

		bool didReadAny = false;

		didReadAny |= ReadJump(ref result);
		didReadAny |= ReadFire(ref result);
		didReadAny |= ReadMoveAxes(ref result);
		didReadAny |= ReadMouseMoveAxes(ref result);

		return didReadAny;
	}

	bool ReadJump(ref PlayerInputState result) {

		if (Input.GetKeyDown(KeyCode.Space)) {

			result.buttonPressedJump = true;
			return true;
		}

		return false;
	}

	bool ReadFire(ref PlayerInputState result) {

		if (Input.GetKeyDown(KeyCode.Mouse0)) {

			result.buttonPressedFire = true;
			return true;
		}

		return false;
	} 

	bool ReadMoveAxes(ref PlayerInputState result) {

		var moveAxes = InputHelper.GetMoveAxes();
		if (moveAxes != Vector2.zero) {

			result.moveAxes = moveAxes;
			return true;
		}

		return false;
	}

	bool ReadMouseMoveAxes(ref PlayerInputState result) {

		var mouseMoveAxes = InputHelper.GetMouseMoveAxes();
		if (mouseMoveAxes != Vector2.zero) {

			result.mouseMoveAxes = mouseMoveAxes;
			return true;
		}

		return false;
	}

	void InsertNewInputRecord(InputEntity playerInputEntity, PlayerInputRecord newInputRecord) {
		
		var inputs = playerInputEntity.playerInputs.inputs;

		var indexToInsert = inputs.FindLastIndex(record => record.timestamp < newInputRecord.timestamp) + 1;

		// If a record with the same timestamp exists, replace it. Otherwise insert.
		if (indexToInsert < inputs.Count && inputs[indexToInsert].timestamp == newInputRecord.timestamp) {
			inputs[indexToInsert] = newInputRecord;
		} else {
			inputs.Insert(indexToInsert, newInputRecord);
		}

		playerInputEntity.ReplacePlayerInputs(inputs);
	}
}

