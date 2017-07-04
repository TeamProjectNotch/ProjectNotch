using System;
using Entitas;

/// Increases the accumulated priority of all Entities with a NetworkUpdatePriorityComponent each simstep.
[SystemAvailability(InstanceKind.Server)]
public class IncreaseNetworkUpdatePriorityInGameContextSystem : IExecuteSystem {

	readonly IGroup<GameEntity> entities;

	public IncreaseNetworkUpdatePriorityInGameContextSystem(Contexts contexts) {

		entities = contexts.game.GetGroup(GameMatcher.NetworkUpdatePriority);
	}

	public void Execute() {

		foreach (var e in entities.GetEntities()) {

			var p = e.networkUpdatePriority;
			e.ReplaceNetworkUpdatePriority(p.basePriority, p.accumulated + p.basePriority);
		}
	}
}
