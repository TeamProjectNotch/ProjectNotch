using System;
using Entitas;

/// Cleans up collision entities that must've been processed already.
public class CleanupCollisionSystem : ICleanupSystem {

	readonly IGroup<EventsEntity> collisions;

	public CleanupCollisionSystem(Contexts contexts) {

		collisions = contexts.events.GetGroup(EventsMatcher.Collision);
	}

	public void Cleanup() {

		collisions.GetEntities().Each(e => e.flagDestroy = true);
	}
}
