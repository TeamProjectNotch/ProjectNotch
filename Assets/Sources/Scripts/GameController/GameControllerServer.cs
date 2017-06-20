using System;
using UnityEngine;
using Entitas;

/// Creates and manages the server-side systems of the game.
public class GameControllerServer : GameControllerBase {

	protected override void CreateSystems(Contexts contexts) {

		systemsFixedUpdate = new ServerFixedTimestepSystems(contexts);
		systemsUpdate = new ServerPerFrameSystems(contexts);
	}
}

