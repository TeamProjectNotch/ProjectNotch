using System;
using Entitas;

/// Ensures that all entities have a NetworkUpdatePriorityComponent.
[SystemAvailability(InstanceKind.Networked)]
public class EnsureNetworkUpdatePrioritySystem : AllEntitiesSystem<INetworkableEntity> {

	const int defaultUpdatePriority = 1;

	readonly IContext[] networkableContexts;

	public EnsureNetworkUpdatePrioritySystem(Contexts contexts) : base(contexts) {}

	protected override void Execute(INetworkableEntity e) {

		if (!e.hasNetworkUpdatePriority) {

			e.AddNetworkUpdatePriority(defaultUpdatePriority, newAccumulated: 0);
		}
	}
}
