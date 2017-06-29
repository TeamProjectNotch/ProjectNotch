using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Entitas;

/// An abstract system.
/// Processes inputs of a player.
/// To use, override Process(GameEntity playerGameEntity, List<PlayerInputRecord> inputs).
/// The second parameter is a non-empty list of input records stretched over a period of time.
/// The idea is that the client might need to resimulate inputs from a particular 
/// moment in the past all the way to the current tick.
public abstract class ProcessInputSystem : ReactiveSystem<InputEntity> {

	readonly GameContext game;
	readonly List<PlayerInputRecord> inputRecordsBuffer = new List<PlayerInputRecord>();

	public ProcessInputSystem(Contexts contexts) : base(contexts.input) {

		game = contexts.game;
	}

	protected override ICollector<InputEntity> GetTrigger(IContext<InputEntity> context) {

		return context.CreateCollector(InputMatcher.ProcessInputs.Added());
	}

	protected override bool Filter(InputEntity e) {

		return e.hasPlayer && e.hasPlayerInputs && e.hasProcessInputs;
	}

	protected override void Execute(List<InputEntity> inputEntities) {

		foreach (var e in inputEntities) Process(e);
	}

	void Process(InputEntity inputEntity) {

		inputRecordsBuffer.Clear();

		var startTick = inputEntity.processInputs.startTick;
		var inputsToProcess = inputEntity.playerInputs.inputs
			.Where(inputRecord => inputRecord.timestamp >= startTick); // TEMP Unoptimized.

		//Debug.LogFormat("Processing input since tick {0} to tick {1}", startTick, game.currentTick.value);

		inputRecordsBuffer.AddRange(inputsToProcess);
		if (inputRecordsBuffer.Count == 0) {
			//Debug.Log("No inputs to process.");
			return;
		}

		var gameEntity = game.GetEntityWithPlayer(inputEntity.player.id);
		if (gameEntity == null) return;

		Process(gameEntity, inputRecordsBuffer);
	}

	PlayerInputRecord GetMostRecentInputRecord(InputEntity inputEntity) {

		var inputs = inputEntity.playerInputs.inputs;
		if (inputs.Count <= 0) return null;

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

		return mostRecentRecord;
	}

	protected abstract void Process(GameEntity player, List<PlayerInputRecord> inputs);
}

/// Removes ProcessInput components from input entities after they have been processed by various ProcessInputSystem|s.
[SystemAvailability(InstanceKind.All)]
public class CleanupProcessInputSystem : ICleanupSystem {

	readonly IGroup<InputEntity> entities;

	public CleanupProcessInputSystem(Contexts contexts) {

		entities = contexts.input.GetGroup(InputMatcher.ProcessInputs);
	}

	public void Cleanup() {

		foreach (var e in entities.GetEntities()) {
			
			e.RemoveProcessInputs();
		}
	}
}
