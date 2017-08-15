using System;

/// Systems that execute with a fixed timestep (in FixedUpdate)
/// An outline of the game loop:
/// 
/// 1. Receive and apply state changes. (Early networking)
/// 2. Game logic, process input, gameobject sync.
/// 3. Compose and send state changes. (Late networking)
/// 4. Destroy entities marked for destruction.
/// 5. Increment tick.
/// 
public class OnFixedUpdateSystems : MyFeature {

	public OnFixedUpdateSystems(Contexts contexts) : base("On FixedUpdate systems") {

        Add(new InitializeNextIdSystem(contexts));

		Add(new EarlyNetworkSystems(contexts));
		Add(new GameLogicSystems(contexts));
        Add(new LateNetworkSystems(contexts));

		Add(new DestroySystem(contexts));
		Add(new TicksSystem(contexts));
	}
}