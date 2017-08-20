using System;
using System.Collections.Generic;
using Entitas;

/// Composes an entity change, given an entity and a connection id of a 
/// network target this change is meant for.
public class EntityChangeComposer {

    public EntityChange Compose(ChangedEntityRecord entityRecord, int connectionId) {

        // TODO Should check if the flags for this particular connectionId are set.
        if (!entityRecord.entity.changeFlags.HasAnyFlagsSet) return null;

        var change = MakeChange(entityRecord);
        return change;
    }

    EntityChange MakeChange(ChangedEntityRecord record) {

        var e = record.entity;
        int contextIndex = record.contextIndex;

        if (record.entity.flagDestroy) {

            return EntityChange.MakeRemoval(record.contextIndex, e.id.value);
        }

        var componentChanges = MakeComponentChangesOf(e);
        return EntityChange.MakeUpdate(contextIndex, e.id.value, componentChanges);
    }

    ComponentChange[] MakeComponentChangesOf(INetworkableEntity e) {

        var componentChanges = new List<ComponentChange>();

        // TODO Use the flags for a particular connectionId.
        var changed = e.changeFlags.flags;
        int numComponents = e.totalComponents;

        for (int i = 0; i < numComponents; ++i) {

            if (!changed[i]) continue;

            var componentChange = e.HasComponent(i) ?
                ComponentChange.MakeUpdate(i, e.GetComponent(i)) :
                ComponentChange.MakeRemoval(i);

            componentChanges.Add(componentChange);
        }

        return componentChanges.ToArray();
    }
}

/*
/// Composes entity changes for a single context
/// for a network message target (client or server),
/// defined by given connectionId.
public interface IContextChangeComposer {

    void Compose(int connectionId, List<EntityChange> changes);
}

public class ContextChangeComposer<TEntity> : IContextChangeComposer
    where TEntity : class, IEntity, INetworkableEntity {

    const int preferredNumBytesPerMessage = 1024;
    const int maxNumEntityChangesPerMessage = 5;

    readonly int contextIndex;
    readonly IContext<TEntity> context;
    readonly IGroup<TEntity> entitiesGroup;

    readonly EntityByPriorityComparer<INetworkableEntity> byPriorityComparer = new EntityByPriorityComparer<INetworkableEntity>();
    readonly List<EntityChange> destroyedEntityChangesBacklog = new List<EntityChange>();

    public ContextChangeComposer(Contexts contexts, int contextIndex) {

        this.contextIndex = contextIndex;
        context = (IContext<TEntity>)contexts.allContexts[contextIndex];

        entitiesGroup = context.GetGroup(Matcher<TEntity>.AllOf(
            context.FindIndexOfComponent<IdComponent>()
        ));
    }

    public void Compose(int connectionId, List<EntityChange> changes) {

        WriteBacklog(changes);

        var entities = entitiesGroup.GetEntities();
        Array.Sort(entities, byPriorityComparer);

        int numChanges = 0;
        for (int i = 0; i < entities.Length; ++i) {

            var e = entities[i];

            // TODO Should check if the flags for this particular connectionId are set.
            if (!e.changeFlags.HasAnyFlagsSet) continue;

            var change = MakeChange(e);
            changes.Add(change);
            numChanges += 1;

            e.UnsetChangeFlags();
            e.ResetNetworkPriority();

            if (i > maxNumEntityChangesPerMessage) {

                AddToBacklog(entities, startingIndex: i);
            }
        }
    }

    void WriteBacklog(List<EntityChange> changes) {

        changes.AddRange(destroyedEntityChangesBacklog);
        destroyedEntityChangesBacklog.Clear();
    }

    /// Adds all Entities getting destroyed to the backlog. 
    /// IMPORTANT: Assumes `entities` is sorted with all the `flagDestroy` entities first.
    void AddToBacklog(TEntity[] entities, int startingIndex) {

        for (int i = startingIndex; i < entities.Length; ++i) {

            var e = entities[i];
            if (!e.flagDestroy) break;

            destroyedEntityChangesBacklog.Add(
                EntityChange.MakeRemoval(contextIndex, e.id.value)
            );
        }
    }

    EntityChange MakeChange(TEntity e) {

        if (e.flagDestroy) {
            
            return EntityChange.MakeRemoval(contextIndex, e.id.value);
        }

        var componentChanges = MakeComponentChangesOf(e);
        return EntityChange.MakeUpdate(contextIndex, e.id.value, componentChanges);
    }

    ComponentChange[] MakeComponentChangesOf(INetworkableEntity e) {

        var componentChanges = new List<ComponentChange>();

        // TODO Use the flags for a particular connectionId.
        var changed = e.changeFlags.flags;
        int numComponents = e.totalComponents;

        for (int i = 0; i < numComponents; ++i) {

            if (!changed[i]) continue;

            var componentChange = e.HasComponent(i) ?
                ComponentChange.MakeUpdate(i, e.GetComponent(i)) :
                ComponentChange.MakeRemoval(i);

            componentChanges.Add(componentChange);
        }

        return componentChanges.ToArray();
    }
}
*/
/// A NetworkUpdatePriority-based comparer of Entities.
/// For sorting in descending order (highest priority first).
/// Entities which are getting destroyed (e.flagDestroy == true) 
/// are automatically highest priority.
public class EntityByPriorityComparer<TEntity> : IComparer<TEntity> 
    where TEntity : IEntity, IDestroy, INetworkUpdatePriority {

    int IComparer<TEntity>.Compare(TEntity a, TEntity b) {

        if (a.flagDestroy && !b.flagDestroy) return -1;
        if (!a.flagDestroy && b.flagDestroy) return  1;

        int priorityA = GetPriorityOf(a);
        int priorityB = GetPriorityOf(b);
        if (priorityA >  priorityB) return -1;
        if (priorityA == priorityB) return  0;
        return 1;
    }

    int GetPriorityOf(TEntity e) {

        return e.hasNetworkUpdatePriority ? e.networkUpdatePriority.accumulated : 0;
    }
}
