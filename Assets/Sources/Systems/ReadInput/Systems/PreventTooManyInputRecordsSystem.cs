using System;
using System.Collections.Generic;
using Entitas;

/// Makes sure that a PlayerInputsComponent never stores too many records.
/// Removes the earliest records to reduce the number of stored records.
public class PreventTooManyInputRecordsSystem : ReactiveSystem<InputEntity> {

	const int maxAllowedInputRecords = 12; // 200ms worth of input records

	public PreventTooManyInputRecordsSystem(Contexts contexts) : base(contexts.input) {}

	protected override ICollector<InputEntity> GetTrigger(IContext<InputEntity> context) {

		return context.CreateCollector(InputMatcher.PlayerInputs.Added());
	}

	protected override bool Filter(InputEntity entity) {

		return entity.hasPlayer && entity.hasPlayerInputs;
	}

	protected override void Execute(List<InputEntity> entities) {

		foreach (var e in entities) {

			var inputRecords = e.playerInputs.inputRecords;
			var numRecords = inputRecords.Count;
			if (numRecords > maxAllowedInputRecords) {
				
				inputRecords.RemoveRange(index: 0, count: numRecords - maxAllowedInputRecords);
				e.ReplacePlayerInputs(inputRecords);
			}
		}
	}
}
