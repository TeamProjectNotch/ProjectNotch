using System;
using System.Linq;
using System.Collections.Generic;
using Entitas;

/// When PlayerInputsComponent is updated with input records, marks those input records 
/// to be processed using ProcessInputComponent.
public class MarkAddedInputsForProcessingSystem : ReactiveSystem<InputEntity> {

	ulong? timestampOfLastProcessedRecord;

	public MarkAddedInputsForProcessingSystem(Contexts contexts) : base(contexts.input) {}

	protected override ICollector<InputEntity> GetTrigger(IContext<InputEntity> context) {

		return context.CreateCollector(InputMatcher.PlayerInputs.Added());
	}

	protected override bool Filter(InputEntity e) {

		return e.hasPlayerInputs;
	}

	protected override void Execute(List<InputEntity> entities) {

		foreach (var e in entities) Process(e);
	}

	void Process(InputEntity e) {

		if (e.playerInputs.inputs.Count == 0) return;

		// TEMP Unoptimized
		var inputRecords = e.playerInputs.inputs
			.Where(inputRecord => 
				inputRecord.timestamp > (timestampOfLastProcessedRecord.HasValue ? timestampOfLastProcessedRecord : 0)
			);
		if (inputRecords.Count() == 0) return;

		var earliestUnprocessedRecordTick = inputRecords.First().timestamp;
		timestampOfLastProcessedRecord = inputRecords.Last().timestamp;

		if (!e.hasProcessInputs || e.processInputs.startTick > earliestUnprocessedRecordTick) {

			e.ReplaceProcessInputs(earliestUnprocessedRecordTick);
		}
	}
}
