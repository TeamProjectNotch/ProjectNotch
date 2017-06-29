using System;
using Entitas;

/// Ensures that all GameEntities in the GameContext have a NetworkUpdatePriorityComponent.
[SystemAvailability(InstanceKind.Server)]
public class EnsureNetworkUpdatePrioritySystem : IInitializeSystem {

	readonly GameContext game;

	const int defaultUpdatePriority = 1;

	public EnsureNetworkUpdatePrioritySystem(Contexts contexts) {

		game = contexts.game;
	}

	public void Initialize() {

		// Add to all existing entities...
		game.GetEntities().Each(AddNetworkUpdatePriotity);
		// ...And all future ones.
		game.OnEntityCreated += (context, entity) => AddNetworkUpdatePriotity(entity);
	}

	void AddNetworkUpdatePriotity(IEntity entity) {

		var e = (GameEntity)entity;
		if (!e.hasNetworkUpdatePriority) {
			
			e.AddNetworkUpdatePriority(defaultUpdatePriority, newAccumulated: 0);
		}
	}
}
