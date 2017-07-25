using System;
using Entitas;

/// Ensures that all networkable entities have a ChangeFlagsComponent.
[SystemAvailability(InstanceKind.Networked)]
public class EnsureChangeFlagsSystem : AllEntitiesSystem<INetworkableEntity> {

	readonly IContext[] networkableContexts;

	public EnsureChangeFlagsSystem(Contexts contexts) : base(contexts) {}

	protected override void Apply(INetworkableEntity e) {

		// Flags all existing components.
		var flags = new bool[e.totalComponents];
		e.GetComponentIndices().Each(i => flags[i] = true);

		e.AddChangeFlags(flags);

	}
}
