using System;

public class GameLogicSystems : MyFeature {

	public GameLogicSystems(Contexts contexts) : base("GameLogic systems") {

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
