using System;
using Entitas;

/// Ensures that all networkable entities have a ChangeFlagsComponent.
[SystemAvailability(InstanceKind.Networked)]
public class EnsureChangeFlagsSystem : IInitializeSystem {

	readonly IContext<INetworkableEntity>[] networkableContexts;

	public EnsureChangeFlagsSystem(Contexts contexts) {

		this.networkableContexts = contexts.GetNetworkableContexts();
	}

	public void Initialize() {

		foreach (var context in networkableContexts) {

			// Add to all existing entities...
			context.GetEntities().Each(AddChangeFlags);
			// ...And all future ones.
			context.OnEntityCreated += (c, entity) => AddChangeFlags(entity);
		}
	}

	void AddChangeFlags(IEntity entity) {

		var e = (INetworkableEntity)entity;

		// Flags all existing components.
		var flags = new bool[e.totalComponents];
		entity.GetComponentIndices().Each(i => flags[i] = true);

		e.AddChangeFlags(flags);
	}
}
