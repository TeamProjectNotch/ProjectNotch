using System;
using Entitas;

/// Ensures that all InputEntities have an IdComponent.
/// TEMP Massive code duplication from EnsureGameEntityIdSystem.
public class EnsureInputEntityIdSystem : IInitializeSystem {

	readonly InputContext input;

	// TEMP. A system shouldn't really store state like this. Need a [Unique]Component.
	ulong nextId = 0;

	public EnsureInputEntityIdSystem(Contexts contexts) {

		input = contexts.input;
	}

	public void Initialize() {
		
		input.GetEntities().Each(AssignId);
		input.OnEntityCreated += (c, entity) => AssignId(entity);
	}

	void AssignId(IEntity entity) {

		var e = (IId)entity;
		e.AddId(nextId);
		nextId += 1;
	} 
}

