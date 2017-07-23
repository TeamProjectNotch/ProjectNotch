using System;
using System.Linq;
using System.Collections.Generic;
using Entitas;

/// Increases the accumulated priority of all entities with a NetworkUpdatePriorityComponent every simstep.
[SystemAvailability(InstanceKind.Networked)]
public class IncreaseNetworkUpdatePrioritySystem : IExecuteSystem {

	readonly IContext[] networkableContexts;

	IEnumerable<INetworkableEntity> entities {
		get {
			
			return networkableContexts.SelectMany(context => context.GetEntities<INetworkableEntity>());
		}
	}

	public IncreaseNetworkUpdatePrioritySystem(Contexts contexts) {

		networkableContexts = contexts.GetNetworkableContexts();
	}

	public void Execute() {

		foreach (var e in entities) {

			if (e.changeFlags.HasAnyFlagsSet) return;
			
			var p = e.networkUpdatePriority;
			e.ReplaceNetworkUpdatePriority(p.basePriority, p.accumulated + p.basePriority);
		}
	}
}
