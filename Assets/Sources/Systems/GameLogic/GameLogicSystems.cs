using Entitas;

public class GameLogicSystems : Feature {

    public GameLogicSystems(Contexts contexts) : base("GameLogic") {

		// Initialize systems
		Add(new CreatePlayerWeaponSystem(contexts));

		// Execute systems
		Add(new ProcessBulletCollisionSystem(contexts));
		Add(new DestroyWhenHealthZeroSystem(contexts));
		Add(new AttachHandheldToPlayerSystem(contexts));

		Add(new TicksSystem(contexts));

		// Cleanup systems
		Add(new CleanupCollisionSystem(contexts));
    }
}
