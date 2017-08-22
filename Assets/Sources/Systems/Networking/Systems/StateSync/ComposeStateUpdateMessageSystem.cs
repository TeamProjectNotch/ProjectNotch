using System;
using System.Linq;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

using NM = NetworkingMatcher;

/// Composes automatic state update messages for network targets 
/// such as clients or servers.
[SystemAvailability(InstanceKind.Networked)]
public class ComposeStateUpdateMessageSystem : IExecuteSystem {

    const int preferredNumBytesPerMessage = 1024;

    readonly Contexts contexts;
    readonly IGroup<NetworkingEntity> messageTargets;

    readonly EntityChangeComposer changeComposer = new EntityChangeComposer();
    readonly SerializedSizeMeasurer sizeMeasurer = new SerializedSizeMeasurer();

    public ComposeStateUpdateMessageSystem(Contexts contexts) {

        this.contexts = contexts;
        messageTargets = GetMessageTargets();
    }

    public void Execute() {

        var targets = messageTargets.GetEntities();

        foreach (var target in targets) {

            var entityChanges = GetEntityChangesFor(target);
            if (entityChanges.Count == 0) continue;

            var message = new StateUpdateMessage() {
                timestamp = contexts.game.currentTick,
                changes = entityChanges.ToArray()
            };

            target.EnqueueOutgoingMessage(message);
            Debug.Log(
                $"ComposeStateUpdateMessageSystem: composed " +
                $"{entityChanges.Count} changes"
            );
        }
    }

    IGroup<NetworkingEntity> GetMessageTargets() {

        if (!ProgramInstance.isClientOrServer) return null;

        var matcher = ProgramInstance.isServer ?
            NM.AllOf(NM.Client, NM.OutgoingMessages) :
            NM.AllOf(NM.Server, NM.OutgoingMessages);

        return contexts.networking.GetGroup(matcher);
    }

    List<EntityChange> GetEntityChangesFor(NetworkingEntity target) {
        
        var changes = new List<EntityChange>();
        int byteLimit = preferredNumBytesPerMessage;
        AddChangesFromBacklog(target, ref byteLimit, changes);
        AddChangesFromRecords(target, ref byteLimit, changes);

        return changes;
    }

    void AddChangesFromBacklog(NetworkingEntity target, ref int byteLimit, List<EntityChange> changes) {

        // Make from backlog
        var backlog = target.destroyedEntitiesBacklog.entityChanges;
        int numProcessedFromBacklog = 0;
        foreach (var change in backlog) {

            int changeSize = sizeMeasurer.Measure(change);
            if (byteLimit < changeSize) break;
            byteLimit -= changeSize;

            numProcessedFromBacklog += 1;
            changes.Add(change);
        }

        // Get processed changes off the backlog.
        var processedChanges = backlog.Take(numProcessedFromBacklog).ToArray();
        backlog.ExceptWith(processedChanges);
        target.ReplaceDestroyedEntitiesBacklog(backlog);
    }

    void AddChangesFromRecords(NetworkingEntity target, ref int byteLimit, List<EntityChange> changes) {

        int connectionId = target.connection.id;

        var records = target
            .entitiesToSend.records
            .OrderBy(r => r.entity, new EntityByPriorityComparer<INetworkableEntity>());
        // TEMP comparer. Use a comparer which takes the connection id into account.

        // Make from records
        int numProcessedRecords = 0;
        foreach (ChangedEntityRecord record in records) {

            var change = changeComposer.Compose(record, connectionId);

            int changeSize = sizeMeasurer.Measure(change);
            if (byteLimit < changeSize) break;
            byteLimit -= changeSize;

            numProcessedRecords += 1;
            record.entity.UnsetChangeFlags();
            record.entity.ResetNetworkPriority();

            changes.Add(change);
        }

        // Add unprocessed items to backlog.
        var destroyedUntrackedEntityChanges = records
            .Skip(numProcessedRecords)
            .TakeWhile(record => record.entity.flagDestroy)
            .Select(record => changeComposer.Compose(record, connectionId));
        
        var backlog = target.destroyedEntitiesBacklog.entityChanges;
        backlog.UnionWith(destroyedUntrackedEntityChanges);
        target.ReplaceDestroyedEntitiesBacklog(backlog);
    }
}

