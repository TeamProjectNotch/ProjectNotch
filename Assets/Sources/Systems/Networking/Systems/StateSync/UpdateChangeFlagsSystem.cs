using System;
using System.Linq;
using System.Collections.Generic;
using Entitas;

// Makes sure that whenever the components of an Entity are changed,
/// its ChangeFlagsComponent is updated accordingly.
[SystemAvailability(InstanceKind.Networked)]
public class UpdateChangeFlagsSystem : IInitializeSystem {

	readonly IContext<INetworkableEntity>[] networkableContexts;
	readonly bool[][] shouldSyncComponent;

	// One per context.
	readonly EntityComponentChanged [] componentAddedHandlers;
	readonly EntityComponentReplaced[] componentReplacedHandlers;
	readonly EntityComponentChanged [] componentRemovedHandlers;

	public UpdateChangeFlagsSystem(Contexts contexts) {

		networkableContexts = contexts.GetNetworkableContexts();
		shouldSyncComponent = ContextSyncInfo.shouldSyncComponent;

		var numContexts = networkableContexts.Length;
		componentAddedHandlers    = new EntityComponentChanged [numContexts];
		componentReplacedHandlers = new EntityComponentReplaced[numContexts];
		componentRemovedHandlers  = new EntityComponentChanged [numContexts];
	}

	/// Makes sure that all current *and* future Entities are kept track of.
	public void Initialize() {

		var numContexts = networkableContexts.Length;
		for (int contextIndex = 0; contextIndex < numContexts; ++contextIndex) {
			
			var context = networkableContexts[contextIndex];

			context.GetEntities().Each(entity => SubscribeToEntityEvents(entity, context, contextIndex));
			context.OnEntityCreated += (ctx, entity) => SubscribeToEntityEvents(entity, ctx, contextIndex);
		}
	}

	/// Makes sure that whenever the given Entity has a component added, replaced, or removed, the changes are recorded.
	void SubscribeToEntityEvents(IEntity entity, IContext context, int contextIndex) {

		EntityComponentChanged onComponentAdded = (e, componentIndex, component) => 
			OnComponentAdded(contextIndex, e, componentIndex);
		componentAddedHandlers[contextIndex] = onComponentAdded;
		entity.OnComponentAdded += onComponentAdded;

		EntityComponentReplaced onComponentReplaced = (e, componentIndex, previousComponent, newComponent) => 
			OnComponentReplaced(contextIndex, e, componentIndex);
		componentReplacedHandlers[contextIndex] = onComponentReplaced;
		entity.OnComponentReplaced += onComponentReplaced;

		EntityComponentChanged onComponentRemoved = (e, componentIndex, component) => 
			OnComponentRemoved(contextIndex, e, componentIndex);
		componentRemovedHandlers[contextIndex] = onComponentRemoved;
		entity.OnComponentRemoved += onComponentRemoved;

		context.OnEntityWillBeDestroyed += (c, e) => OnEntityWillBeDestroyed(contextIndex, e);
	}

	void OnComponentAdded(int contextIndex, IEntity e, int componentIndex) {
		
		SetChangedFlag(contextIndex, (INetworkableEntity)e, componentIndex);
	}

	void OnComponentReplaced(int contextIndex, IEntity e, int componentIndex) {
		
		SetChangedFlag(contextIndex, (INetworkableEntity)e, componentIndex);
	}

	void OnComponentRemoved(int contextIndex, IEntity e, int componentIndex) {
		
		SetChangedFlag(contextIndex, (INetworkableEntity)e, componentIndex);
	}

	void OnEntityWillBeDestroyed(int contextIndex, IEntity e) {

		// So that it doesn't get pointlessly called when the entity removes all of its components during destruction.
		e.OnComponentRemoved -= componentRemovedHandlers[contextIndex];
	}

	void SetChangedFlag(int contextIndex, INetworkableEntity entity, int componentIndex) {

		if (shouldSyncComponent[contextIndex][componentIndex]) {

			entity.changeFlags.flags[componentIndex] = true;
		}
	}
}
