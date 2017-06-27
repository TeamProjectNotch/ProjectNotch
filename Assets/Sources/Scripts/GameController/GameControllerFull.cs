using System;
using Entitas;

/// A singular game controller configurable to work as a server or a client.
public class GameControllerFull : GameControllerBase {

	public InstanceKind thisInstanceKind;

	void Awake() {

		Contexts.sharedInstance.gameState.SetProgramInstanceKind(thisInstanceKind);
	}

	protected override void CreateSystems(Contexts contexts) {

		// TEMP. This configuration happens to contain all systems. For now. Should create them differently.
		systemsFixedUpdate = new Feature("On FixedUpdate").Add(new AllFixedTimestepSystems(contexts));
		systemsUpdate = new Feature("On Update").Add(new ClientPerFrameSystems(contexts));
	}
}
