using System;
using Entitas;

/// Ensures that all entities have a NetworkUpdatePriorityComponent.
[SystemAvailability(InstanceKind.Networked)]
public class EnsureNetworkUpdatePrioritySystem : IInitializeSystem {

	const int defaultUpdatePriority = 1;

	readonly IContext<INetworkableEntity>[] networkableContexts;

	public EnsureNetworkUpdatePrioritySystem(Contexts contexts) {

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
		if (!e.hasNetworkUpdatePriority) {

			e.AddNetworkUpdatePriority(defaultUpdatePriority, newAccumulated: 0);
		}
	}
}
