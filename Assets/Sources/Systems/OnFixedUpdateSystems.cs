using System;

/// Systems that execute with a fixed timestep (in FixedUpdate)
/// A outline of the game loop:
/// 
/// 1. Receive and apply state changes.
/// 2. Game logic, process input, gameobject sync.
/// 3. Assign ids to entities created in the previous step.
/// 4. Compose and send state changes.
/// 5. Destroy entities marked for destruction.
/// 6. Increment tick.
/// 
public class OnFixedUpdateSystems : MyFeature {

	public OnFixedUpdateSystems(Contexts contexts) : base("On FixedUpdate systems") {

        Add(new InitializeNextIdSystem(contexts));
        //Add(new UpdateNextIdSystem(contexts));

		Add(new EarlyNetworkSystems(contexts));
		Add(new GameLogicSystems(contexts));
        Add(new AssignEntityIdSystem(contexts));
        Add(new LateNetworkSystems(contexts));

		Add(new DestroySystem(contexts));
		Add(new TicksSystem(contexts));
	}
}