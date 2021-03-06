﻿using System;

/// Systems that execute with a fixed timestep (in FixedUpdate)
/// A outline of the game loop:
/// Network receive
/// Read input (in update)
/// Game logic & process input & gameobject sync
/// Destroy entities marked for destruction
/// Increment tick.
public class OnFixedUpdateSystems : MyFeature {

	public OnFixedUpdateSystems(Contexts contexts) : base("On FixedUpdate systems") {

		Add(new EnsureGameEntityIdSystem(contexts));
		Add(new EnsureInputEntityIdSystem(contexts));

		Add(new NetworkingSystems(contexts));
		Add(new AllGameLogicSystems(contexts));
		Add(new SendQueuedMessagesSystem(contexts));

		Add(new DestroySystem(contexts));
		Add(new TicksSystem(contexts));
	}
}
