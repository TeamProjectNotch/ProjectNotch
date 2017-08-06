using System;
using System.Collections.Generic;
using System.Linq;
using Entitas;

/// Assigns ids to entities which don't have them.
public class AssignEntityIdSystem : IExecuteSystem {

    readonly Contexts contexts;

    readonly List<IId> createdEntitiesBuffer = new List<IId>();

    public AssignEntityIdSystem(Contexts contexts) {

        this.contexts = contexts;
        
        contexts.allContexts
            .Where(c => c.EntityIs<IId>())
            .Each(c => c.OnEntityCreated += OnEntityCreatedHandler);
    }

    void OnEntityCreatedHandler(IContext context, IEntity entity) {

        createdEntitiesBuffer.Add((IId)entity);
    }

    public void Execute() {
        
        foreach (IId entity in createdEntitiesBuffer) {

            if (!entity.hasId) {

                contexts.AssignId(entity);
            }
        }

        createdEntitiesBuffer.Clear();
    }
}