using System;
using System.Linq;
using System.Collections.Generic;
using Entitas;

/// Commands a player input entity to process inputs added since last Execute.
/// Does so by adding a `ProcessInputComponent`.
[SystemAvailability(InstanceKind.All)]
public class MarkAddedInputsForProcessingSystem : IExecuteSystem {

	readonly GameContext game;
	readonly IGroup<InputEntity> playerInputEntities;

	ulong? timestampOfLastProcessedRecord;

	public MarkAddedInputsForProcessingSystem(Contexts contexts) {

		game = contexts.game;

		playerInputEntities = contexts.input.GetGroup(
			InputMatcher.AllOf(InputMatcher.Player, InputMatcher.PlayerInputs)
		);
	}

	public void Execute() {

		foreach (var e in playerInputEntities.GetEntities()) {

			Process(e);
		}
	}

	void Process(InputEntity e) {

		var inputRecords = GetUnprocessedInputRecords(e.playerInputs.inputRecords);
		bool recordsAvailable = (inputRecords.Count() > 0);

		var earliestUnprocessedRecordTick = recordsAvailable ? inputRecords.First().timestamp : game.currentTick.value;

		if (recordsAvailable) {
			timestampOfLastProcessedRecord = inputRecords.Last().timestamp;
		}

		if (!e.hasProcessInputs || e.processInputs.startTick > earliestUnprocessedRecordTick) {

			e.ReplaceProcessInputs(earliestUnprocessedRecordTick);
		}

		//UnityEngine.Debug.LogFormat("earliestUnprocessedRecordTick: {0}", earliestUnprocessedRecordTick);
	}

	IEnumerable<PlayerInputRecord> GetUnprocessedInputRecords(List<PlayerInputRecord> records) {
		
		// TEMP unoptimized.
		var inputRecords = records
			.Where(inputRecord => 
				inputRecord.timestamp > timestampOfLastProcessedRecord.GetValueOrDefault(0)
			);

		return inputRecords;
	}
}
