using System;
using Entitas;

/// Systems that process InputEntities in InputContexts.
public class ProcessInputSystems : MyFeature {

	public ProcessInputSystems(Contexts contexts) : base("ProcessInput") {

		Add(new ResetPlayerInputSystem(contexts));

		Add(new MarkAddedInputsForProcessingSystem(contexts));

		// ProcessInputSystem|s
		Add(new ProcessMovementInputSystem(contexts));
		Add(new ShootWeaponOnPressFireSystem(contexts));

		Add(new CleanupProcessInputSystem(contexts));
	}
}

