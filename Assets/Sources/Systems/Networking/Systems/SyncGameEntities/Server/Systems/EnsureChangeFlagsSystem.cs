using System;
using Entitas;

/// Ensures that all GameEntities in the GameContext have a ChangeFlagsComponent.
[SystemAvailability(InstanceKind.Server | InstanceKind.Client)]
public class EnsureChangeFlagsSystem : IInitializeSystem {

	readonly GameContext game;

	public EnsureChangeFlagsSystem(Contexts contexts) {

		game = contexts.game;
	}

	public void Initialize() {

		// Add to all existing entities...
		game.GetEntities().Each(AddChangeFlags);
		// ...And all future ones.
		game.OnEntityCreated += (context, entity) => AddChangeFlags(entity);
	}

	void AddChangeFlags(IEntity entity) {
		
		var e = (GameEntity)entity;

		// Flags all existing components.
		var flags = new bool[e.totalComponents];
		entity.GetComponentIndices().Each(i => flags[i] = true);

		e.AddChangeFlags(flags);
	}
}
