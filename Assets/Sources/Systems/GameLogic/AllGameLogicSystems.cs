using System;

public class AllGameLogicSystems : MyFeature {

	public AllGameLogicSystems(Contexts contexts) : base("All game logic systems") {

		// Initialize systems
		Add(new CreatePlayerWeaponSystem(contexts));
		Add(new TestCreateMonitorEntitySystem(contexts));
		// Initialize systems

		// Execute systems
		Add(new ProcessInputSystems(contexts));

		Add(new ProcessBulletCollisionSystem(contexts));
		Add(new DestroyWhenHealthZeroSystem(contexts));
		Add(new TestScreenBufferSystem(contexts));

		Add(new GameObjectSystems(contexts));
		// Execute systems

		// Cleanup systems
		Add(new CleanupCollisionSystem(contexts));
		// Cleanup systems
	}
}
