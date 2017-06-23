using System;
using System.Collections.Generic;
using UnityEngine;
using Entitas;

/// An abstract system.
/// Processes inputs of a player.
/// To use, override Process(GameEntity playerGameEntity, List<PlayerInputRecord> inputs).
/// The second parameter is a non-empty list of input records stretched over a period of time.
/// The idea is that the client might need to resimulate inputs from a particular 
/// moment in the past all the way to the current tick.
/// TEMP The list only contains the last recorded input record.
public abstract class ProcessInputSystem : ReactiveSystem<InputEntity> {

	readonly GameContext game;
	readonly InputContext input;

	readonly IGroup<GameEntity> players;

	readonly List<PlayerInputRecord> inputRecordsBuffer = new List<PlayerInputRecord>();

	// TEMP This here is a dirty hack that prevents the system from processing the same input over and over again.
	// The issue exists because input entities are not synced between clients and servers properly. 
	// See ComposeInputMessageSystem. Can be solved by creating 
	// a generic abstract system that would compose messages to send over the network for any given context.
	ulong? timestampOfLastProcessedInput;

	public ProcessInputSystem(Contexts contexts) : base(contexts.input) {

		game = contexts.game;
		input = contexts.input;

		players = game.GetGroup(
			GameMatcher.AllOf(GameMatcher.Player, GameMatcher.GameObject)
		);
	}

	protected override ICollector<InputEntity> GetTrigger(IContext<InputEntity> context) {

		return context.CreateCollector(InputMatcher.PlayerInputs.Added());
	}

	protected override bool Filter(InputEntity entity) {

		return entity.hasPlayer && entity.hasPlayerInputs;
	}

	protected override void Execute(List<InputEntity> inputEntities) {

		foreach (var e in inputEntities) Process(e);
	}

	void Process(InputEntity inputEntity) {

		var mostRecentRecord = GetMostRecentInputRecord(inputEntity);
		if (mostRecentRecord == null) return;
		if (mostRecentRecord.timestamp == timestampOfLastProcessedInput) return;

		inputRecordsBuffer.Clear();
		inputRecordsBuffer.Add(mostRecentRecord);

		var gameEntity = game.GetEntityWithPlayer(inputEntity.player.id);

		Process(gameEntity, inputRecordsBuffer);

		timestampOfLastProcessedInput = mostRecentRecord.timestamp;
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
