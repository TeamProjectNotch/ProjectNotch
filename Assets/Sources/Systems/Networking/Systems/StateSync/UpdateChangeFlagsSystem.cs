using System;
using System.Linq;
using System.Collections.Generic;
using Entitas;

// Makes sure that whenever the components of an Entity are changed,
/// its ChangeFlagsComponent is updated accordingly.
[SystemAvailability(InstanceKind.Networked)]
public class UpdateChangeFlagsSystem : IInitializeSystem {

	readonly Contexts contexts;
	readonly bool[][] shouldSyncComponent;

	// One per context.
	EntityComponentChanged [] componentAddedHandlers;
	EntityComponentReplaced[] componentReplacedHandlers;
	EntityComponentChanged [] componentRemovedHandlers;

	public UpdateChangeFlagsSystem(Contexts contexts) {

		this.contexts = contexts;
		shouldSyncComponent = ContextSyncInfo.shouldSyncComponent;
	}
		
	public void Initialize() {

		var allContexts = contexts.allContexts;
		var numContexts = allContexts.Length;

		componentAddedHandlers    = new EntityComponentChanged [numContexts];
		componentReplacedHandlers = new EntityComponentReplaced[numContexts];
		componentRemovedHandlers  = new EntityComponentChanged [numContexts];

		for (int contextIndex = 0; contextIndex < numContexts; ++contextIndex) {

			var context = allContexts[contextIndex];
            if (!context.IsNetworkable()) continue;

			InitializeEntityEventsHandlersFor(contextIndex);

			// Copy the variable as a way force lambdas to capture it by value.
			int contextIndexCopy = contextIndex;
			context.GetEntities<INetworkableEntity>().Each(entity => SubscribeToEntityEvents(entity, context, contextIndexCopy));
			context.OnEntityCreated += (ctx, entity) => SubscribeToEntityEvents(entity, ctx, contextIndexCopy);
		}
	}

	void InitializeEntityEventsHandlersFor(int contextIndex) {

		componentAddedHandlers[contextIndex] = 
			(e, componentIndex, c) => OnComponentAdded(contextIndex, e, componentIndex);

		componentReplacedHandlers[contextIndex] = 
			(e, componentIndex, pc, nc) => OnComponentReplaced(contextIndex, e, componentIndex);

		componentRemovedHandlers[contextIndex] = 
			(e, componentIndex, c) => OnComponentRemoved(contextIndex, e, componentIndex);
	}

	/// Makes sure that whenever the given Entity has a component added, replaced, or removed, the changes are recorded.
	void SubscribeToEntityEvents(IEntity entity, IContext context, int contextIndex) {
		
		entity.OnComponentAdded    += componentAddedHandlers   [contextIndex];
		entity.OnComponentReplaced += componentReplacedHandlers[contextIndex];
		entity.OnComponentRemoved  += componentRemovedHandlers [contextIndex];

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
