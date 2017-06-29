using System;
using Entitas;

/// Synchronizes Entities and their GameObjects (their Transform, PhysicsState etc.)
public class SyncGameObjectsAndEntitiesSystems : MyFeature {

	readonly Systems updateGameObjectsSystems;

	public SyncGameObjectsAndEntitiesSystems(Contexts contexts) : base("SyncGameObjectsAndEntities") {

		updateGameObjectsSystems = new UpdateGameObjectsSystems(contexts);
		Add(updateGameObjectsSystems);
		Add(new UpdateGameObjectDrivenEntitiesSystem(contexts));
	}

	public override void Execute() {

		base.Execute();
		// So that any changes UpdateGameObjectDrivenEntitiesSystem just made aren't recorded.
		updateGameObjectsSystems.ClearReactiveSystems();
	}
}
