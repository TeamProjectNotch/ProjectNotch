using System;
using UnityEngine;
using Entitas;

/// Creates and manages ALL the systems of the game. For singleplayer, without clients/servers, just all in one instance.
public class GameControllerUnified : GameControllerBase {

	protected override void CreateSystems(Contexts contexts) {

		// TEMP. This configuration happens to contain all systems. For now. Should create them differently.
		systemsFixedUpdate = new ServerFixedTimestepSystems(contexts);
		systemsUpdate = new ClientPerFrameSystems(contexts);
	}
}
