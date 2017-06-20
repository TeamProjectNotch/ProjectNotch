using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Creates a simple entity with health.
public class EntityCreatorHealthySimple : EntityCreatorSimple {

    public float Health = 100f;

	public override GameEntity CreateEntity(Contexts contexts) {

		var e = base.CreateEntity(contexts);

        e.AddHealth(Health, Health);

		return e;
	}
}
