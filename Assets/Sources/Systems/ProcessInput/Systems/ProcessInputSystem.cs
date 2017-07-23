using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Entitas;

/// An abstract system.
/// Processes inputs of a player. Processes only those inputs that are older that the timestamp 
/// specified in the ProcessInputsComponent of the entity.
/// To use, override Process(GameEntity playerGameEntity, List<PlayerInputRecord> inputs).
/// The second parameter is a possibly empty list of input records stretched over a period of time.
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

		var gameEntity = game.GetEntityWithPlayer(inputEntity.player.id);
		if (gameEntity == null) return;

		inputRecordsBuffer.Clear();

		var startTick = inputEntity.processInputs.startTick;
		var currentTick = game.currentTick.value;
		var inputsToProcess = inputEntity.playerInputs.inputs
			.Where(inputRecord => inputRecord.timestamp >= startTick && inputRecord.timestamp <= currentTick); // TEMP Unoptimized.

		//Debug.LogFormat("Processing input since tick {0} to tick {1}", startTick, game.currentTick.value);

		inputRecordsBuffer.AddRange(inputsToProcess);
		Process(gameEntity, inputRecordsBuffer);
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
