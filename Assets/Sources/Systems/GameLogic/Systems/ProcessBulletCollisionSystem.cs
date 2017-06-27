using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entitas;

/// Currenly destroys the bullet when it hits anything.
[SystemAvailability(InstanceKind.Server | InstanceKind.Singleplayer)]
public class ProcessBulletCollisionSystem : ReactiveSystem<EventsEntity> {

	readonly GameContext game;

	public ProcessBulletCollisionSystem(Contexts contexts) : base(contexts.events) {

		game = contexts.game;
	}

	protected override ICollector<EventsEntity> GetTrigger(IContext<EventsEntity> context) {

		return context.CreateCollector(EventsMatcher.Collision);
	}

	protected override bool Filter(EventsEntity entity) {

		return true;
	}

	protected override void Execute(List<EventsEntity> collisions) {

		foreach (var e in collisions) Process(e);
	}

	void Process(EventsEntity e) {

		var collision = e.collision;

		var self = game.GetEntityWithId(collision.selfEntityId);
		if (self == null) return;
		if (!self.isBullet) return;

		self.flagDestroy = true;

		var other = game.GetEntityWithId(collision.otherEntityId);
        if(!other.hasHealth) return;
		//Debug.LogFormat("Hit! {0} -> {1}", self, other);
        
		other.ReplaceHealth(other.health.health - self.damage.value, other.health.maxHealth);
	}
}