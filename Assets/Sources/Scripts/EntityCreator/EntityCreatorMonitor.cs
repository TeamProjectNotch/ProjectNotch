using System;
using UnityEngine;

/// Creates the screen Entity the GameObject of this script would represent.
/// The created entity has a screen buffer with the same resolution as 
/// the ScreenView found on the same GameObject as this script.
public class EntityCreatorMonitor : EntityCreator {

	public override GameEntity CreateEntity(Contexts contexts) {

		var screenView = GetComponentInChildren<ScreenView>();
		var screenResolution = screenView.resolution;

		var transformState = transform.GetState();
		var screenState = new ScreenBufferState(screenResolution);
		var e = contexts.game.CreateMonitor(transformState, screenState);

		return e;
	}
}
