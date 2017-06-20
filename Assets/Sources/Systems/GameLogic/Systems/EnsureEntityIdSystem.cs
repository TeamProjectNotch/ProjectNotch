using System;
using Entitas;

/// Ensures that all GameEntities have an IdComponent.
public class EnsureEntityIdSystem : IInitializeSystem {

	readonly GameContext game;

	// TEMP. A system shouldn't really store state like this. Need a [Unique]Component.
	ulong nextId = 0;

	public EnsureEntityIdSystem(Contexts contexts) {

		game = contexts.game;
	}

	public void Initialize() {

		game.GetEntities().Each(AssignId);
		game.OnEntityCreated += (context, entity) => AssignId(entity);
	}

	void AssignId(IEntity entity) {
		
		var e = (GameEntity)entity;
		e.AddId(nextId);
		nextId += 1;
	} 
}
