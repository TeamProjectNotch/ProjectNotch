using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityCreatorDCPU : EntityCreator {
    public override GameEntity CreateEntity(Contexts contexts) {
        var e = contexts.game.CreateEntity();
        ContextsIdExtensions.AssignId(contexts, e);
        e.AddDCPU(new dcpuState());
        return e;
    }

    void Start () {
		
	}
}
