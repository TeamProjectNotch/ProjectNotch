using System;
using Entitas;

/// Systems that process InputEntities in InputContexts.
public class ProcessInputSystems : Feature {

	public ProcessInputSystems(Contexts contexts) : base("ProcessInput") {

		Add(new ResetPlayerInputSystem(contexts));

		Add(new ShootWeaponOnPressFireSystem(contexts));
		Add(new ProcessMovementInputSystem(contexts));
	}
}

