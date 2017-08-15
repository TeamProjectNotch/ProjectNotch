using System;
using System.Collections.Generic;
using System.Linq;
using Entitas;

/// A system which updates the EntitiesToSendComponent on entities which represent
/// connections with clients or servers. When an entity has its change flags updated,
/// adds or removes it from appropriate sets of entities to be sent.
/// There has to be a system per networkable context.
[SystemAvailability(InstanceKind.Networked)]
public class UpdateEntitiesToSendSystem<TEntity> : ReactiveSystem<TEntity> 
    where TEntity : class, IEntity, INetworkableEntity {

    readonly int contextIndex;
    readonly IGroup<NetworkingEntity> connectionsGroup;

    public UpdateEntitiesToSendSystem(
        Contexts contexts, IContext<TEntity> context
    ) : base(context) {

        contextIndex = contexts.allContexts.IndexOf(context);

		connectionsGroup = contexts.networking.GetGroup(
			NetworkingMatcher.Connection
		);
    }

    protected override ICollector<TEntity> GetTrigger(IContext<TEntity> context) {

        var matcher = Matcher<TEntity>.AllOf(
            context.FindIndexOfComponent<ChangeFlagsComponent>()
        );
        return context.CreateCollector(matcher.AddedOrRemoved());
    }

    protected override bool Filter(TEntity entity) {

		return entity.hasId;
	}

	protected override void Execute(List<TEntity> entities) {

		foreach (var connection in connectionsGroup.GetEntities()) {

			var changedEntityRecords = connection.entitiesToSend.records;
			int connectionId = connection.connection.id;

			foreach (var e in entities) {

				var record = new ChangedEntityRecord {
					entity = e,
					contextIndex = contextIndex
				};

				if (NeedsSending(e, connectionId)) {
					changedEntityRecords.Add(record);
				} else {
					changedEntityRecords.Remove(record);
				}
			}

			connection.ReplaceEntitiesToSend(changedEntityRecords);
		}
	}

	bool NeedsSending(INetworkableEntity e, int connectionId) {

		// TEMP Doesn't take into account connection-specific flags.
		return e.hasChangeFlags && e.changeFlags.HasAnyFlagsSet;
	}
}

/// See UpdateEntitiesToSendSystem (without the 's').
/// Creates one ^^^ for each networkable context.
[SystemAvailability(InstanceKind.Networked)]
public class UpdateEntitiesToSendSystems : MyFeature {

    public UpdateEntitiesToSendSystems(Contexts contexts)
        : base("UpdateEntitiesToSend systems") {

        MakeSystems(contexts);
    }

    void MakeSystems(Contexts contexts) {

		foreach (var context in contexts.GetNetworkableContexts()) {

			var entityType = context.GetEntityType();
			var systemType = typeof(UpdateEntitiesToSendSystem<>)
				.MakeGenericType(entityType);

			var system = (ISystem)Activator.CreateInstance(
				systemType,
				new object[] { contexts, context }
			);

			Add(system);
		}
    }
}
