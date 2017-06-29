using System;
using System.Linq;
using System.Collections.Generic;
using Entitas;

/// Makes sure that whenever the components of an Entity are changed,
/// its ChangeFlagsComponent is updated accordingly.
[SystemAvailability(InstanceKind.Server)]
public class UpdateChangeFlagsSystem : IInitializeSystem {

	// The indices of components that shouldn't be tracked.
	readonly int[] excludedComponentIndices = {
		GameComponentsLookup.ChangeFlags,
		GameComponentsLookup.NetworkUpdatePriority,
		GameComponentsLookup.GameObject
	};

	readonly GameContext game;

	public UpdateChangeFlagsSystem(Contexts contexts) {
		
		game = contexts.game;
	}

	/// Makes sure that all current *and* future Entities are kept track of.
	public void Initialize() {

		game.GetEntities().Each(SubscribeToEntityEvents);
		game.OnEntityCreated += (context, entity) => SubscribeToEntityEvents(entity);
	}

	/// Makes sure that whenever the given Entity has a component added, replaced, or removed, the changes are recorded.
	void SubscribeToEntityEvents(IEntity e) {

		e.OnComponentAdded += OnComponentAdded;
		e.OnComponentReplaced += OnComponentReplaced;
		e.OnComponentRemoved += OnComponentRemoved;

		game.OnEntityWillBeDestroyed += OnEntityWillBeDestroyed;
	}

	void OnComponentAdded(IEntity e, int componentIndex, IComponent component) {
		SetChangedFlag((GameEntity)e, componentIndex);
	}

	void OnComponentReplaced(IEntity e, int componentIndex, IComponent previousComponent, IComponent newComponent) {
		SetChangedFlag((GameEntity)e, componentIndex);
	}

	void OnComponentRemoved(IEntity e, int componentIndex, IComponent component) {
		SetChangedFlag((GameEntity)e, componentIndex);
	}

	void OnEntityWillBeDestroyed(IContext context, IEntity e) {

		// So that it doesn't get pointlessly called when the entity removes all of its components during destruction.
		e.OnComponentRemoved -= OnComponentRemoved;
	}

	void SetChangedFlag(GameEntity entity, int componentIndex) {

		if (excludedComponentIndices.Contains(componentIndex)) return;
		entity.changeFlags.flags[componentIndex] = true;
	}
}
