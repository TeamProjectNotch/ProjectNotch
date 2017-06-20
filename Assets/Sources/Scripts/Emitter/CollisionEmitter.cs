using System;
using UnityEngine;
using Entitas;
using Entitas.Unity;

/// Emits a collision Entity whenever it collides with a GameObject that also belongs to an Entity.
public class CollisionEmitter : MonoBehaviour {
	
	Contexts contexts;

	EntityLink _thisLink;
	EntityLink thisLink {
		get {

			return _thisLink ?? (_thisLink = gameObject.GetEntityLink());
		}
	}

	void Awake() {

		contexts = Contexts.sharedInstance;
	}

	void OnCollisionEnter(Collision collision) {

		if (thisLink == null) return;

		var otherLink = collision.gameObject.GetComponentInParent<EntityLink>();
		if (otherLink == null) return;

		var thisEntity  = thisLink.entity  as GameEntity;
		if (thisEntity == null) return;

		var otherEntity = otherLink.entity as GameEntity;
		if (otherEntity == null) return;

		contexts.events.CreateEntity()
			.AddCollision(thisEntity.id.value, otherEntity.id.value);
	}
}
