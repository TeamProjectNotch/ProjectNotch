using System;
using System.Collections.Generic;
using Entitas;

/// Increases the accumulated priority of all entities with a NetworkUpdatePriorityComponent each simstep.
[SystemAvailability(InstanceKind.Networked)]
public class IncreaseNetworkUpdatePrioritySystem : MultiReactiveSystem<INetworkableEntity, Contexts> {
	
	readonly IGroup<GameEntity> entities;

	public IncreaseNetworkUpdatePrioritySystem(Contexts contexts) : base(contexts) {

		entities = contexts.game.GetGroup(GameMatcher.NetworkUpdatePriority);
	}

	protected override ICollector[] GetTrigger(Contexts contexts) {

		var collectors = new List<ICollector>();

		var networkableContexts = contexts.GetNetworkableContexts();
		foreach (var context in networkableContexts) {

			var componentIndex = context.FindIndexOfComponent<ChangeFlagsComponent>();
			var matcher = Matcher<INetworkableEntity>.AllOf(componentIndex);

			var collector = context.CreateCollector(matcher.Added());
			collectors.Add(collector);
		}

		return collectors.ToArray();
	}

	protected override bool Filter(INetworkableEntity e) {

		return e.hasNetworkUpdatePriority && e.hasChangeFlags && e.changeFlags.HasAnyFlagsSet;
	}

	public void Execute() {

		foreach (var e in entities.GetEntities()) {
			
			var p = e.networkUpdatePriority;
			e.ReplaceNetworkUpdatePriority(p.basePriority, p.accumulated + p.basePriority);
		}
	}

	protected override void Execute(List<INetworkableEntity> entities) {
		
		foreach (var e in entities) {

			var p = e.networkUpdatePriority;
			e.ReplaceNetworkUpdatePriority(p.basePriority, p.accumulated + p.basePriority);
		}
	}
}
