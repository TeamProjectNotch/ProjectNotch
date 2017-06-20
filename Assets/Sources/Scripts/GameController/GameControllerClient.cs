using System;

public class GameControllerClient : GameControllerBase {

	protected override void CreateSystems(Contexts contexts) {

		systemsFixedUpdate = new ClientFixedTimestepSystems(contexts);
		systemsUpdate = new ClientPerFrameSystems(contexts);
	}
}

