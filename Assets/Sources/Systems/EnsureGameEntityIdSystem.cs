using System;
using Entitas;

/// Ensures that all GameEntities have an IdComponent.
public class EnsureGameEntityIdSystem : IInitializeSystem {

	readonly GameContext game;

	// TEMP. A system shouldn't really store state like this. Need a [Unique]Component.
	ulong nextId = 0;

	public EnsureGameEntityIdSystem(Contexts contexts) {

		game = contexts.game;
	}

	public void Initialize() {

		game.GetEntities().Each(AssignId);
		game.OnEntityCreated += (c, entity) => AssignId(entity);
	}

	void AssignId(IEntity entity) {
		
		var e = (IId)entity;
		e.AddId(nextId);
		nextId += 1;
	} 
}
