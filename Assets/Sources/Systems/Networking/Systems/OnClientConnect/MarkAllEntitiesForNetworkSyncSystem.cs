using System;
using System.Linq;
using System.Collections.Generic;
using Entitas;

[SystemAvailability(InstanceKind.Server)]
public class MarkAllEntitiesForNetworkSync : ReactiveSystem<NetworkingEntity> {

    readonly int[] networkableContextIndices;
    readonly Func<INetworkableEntity[]>[] entitiesGetters;

    public MarkAllEntitiesForNetworkSync(Contexts contexts) : base(contexts.networking) {

        networkableContextIndices = Enumerable
            .Range(0, contexts.allContexts.Length)
            .Where(index => contexts.allContexts[index].IsNetworkable())
            .ToArray();

        entitiesGetters = contexts
            .allContexts
            .Select(c => c.IsNetworkable() ? MakeEntitiesGetter(c) : null)
            .ToArray();
    }

    protected override ICollector<NetworkingEntity> GetTrigger(IContext<NetworkingEntity> context) {

        return context.CreateCollector(NetworkingMatcher.Client);
    }

    protected override bool Filter(NetworkingEntity entity) { return true; }

    protected override void Execute(List<NetworkingEntity> newClients) {

        foreach (var client in newClients) {

            MarkEntitiesToBeNetworkedTo(client);
        }
    }

    Func<INetworkableEntity[]> MakeEntitiesGetter(IContext context) {

        return (Func<INetworkableEntity[]>)Delegate.CreateDelegate(
            typeof(Func<INetworkableEntity[]>), 
            context, "GetEntities"
        );
    }

    void MarkEntitiesToBeNetworkedTo(NetworkingEntity client) {
        
        foreach (var contextIndex in networkableContextIndices) {

            var entities = entitiesGetters[contextIndex]();
            var shouldSyncComponentMap = ContextSyncInfo.shouldSyncComponent[contextIndex];

            foreach (var e in entities) {

                var flags = e.changeFlags.flags;

                // Marks all components the entity has for sending.
                e.GetComponentIndices()
                 .Each(i => flags[i] = shouldSyncComponentMap[i]);

                e.ReplaceChangeFlags(flags);
            }
        }
    }

}
