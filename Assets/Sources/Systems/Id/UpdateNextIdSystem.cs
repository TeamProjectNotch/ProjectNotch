using System;
using System.Linq;
using System.Reflection;
using Entitas;

/// Every time an id is assigned to an entity, checks if this new id is bigger than game.nextId . 
/// If so, updates game.nextId to make sure it's always bigger than any already used id.
public class UpdateNextIdSystem : IInitializeSystem {

    readonly Contexts contexts;
    readonly GameContext game;

    public UpdateNextIdSystem(Contexts contexts) {

        this.contexts = contexts;
        this.game = contexts.game;
    }

    public void Initialize() {

        contexts.allContexts
            .Where(context => context.EntityIs<IId>())
            .Each(context => {

                var method = GetHandlerFor(context);
                var parameters = new[] {context};
                method.Invoke(this, parameters);
            });
    }

    MethodInfo GetHandlerFor(IContext context) {

        return this.GetType()
            .GetMethod("ProcessContext", BindingFlags.NonPublic | BindingFlags.Instance)
            .MakeGenericMethod(context.GetEntityType());
    }

    /// Subscribes to events in a given context (must have IdComponent defined in it.)
    void ProcessContext<TEntity>(IContext<TEntity> context) where TEntity : class, IEntity, IId {

        var matcher = Matcher<TEntity>.AllOf(context.FindIndexOfComponent<IdComponent>());
        context.GetGroup(matcher).OnEntityAdded += (group, entity, index, component) => {

            var entityId = ((IdComponent)component).value;
            IdAssigned(entityId);
        };
    }

    void IdAssigned(ulong entityId) {

        if (entityId >= game.nextId.value) {

            game.ReplaceNextId(entityId + 1ul);
        }
    }
}
