using System;
using System.Linq;
using System.Collections.Generic;
using Entitas;
using System.IO;

[SystemAvailability(InstanceKind.Networked)]
public class ComposeStateUpdateMessageSystem : IExecuteSystem {

	const int preferredNumBytesPerMessage = 1024;
	const int maxNumEntityChangesPerMessage = 5;

	readonly Contexts contexts;
	readonly IContext[] networkableContexts;
	readonly IGroup<NetworkingEntity> messageTargets;

	readonly EntityByPriorityComparer<INetworkableEntity> byPriorityComparer = new EntityByPriorityComparer<INetworkableEntity>();
	readonly List<EntityChange> destroyedEntityChangesBacklog = new List<EntityChange>();

	IEnumerable<INetworkableEntity> entitiesToTrack {
		get {
			return networkableContexts.SelectMany(context => context.GetEntities<INetworkableEntity>());
		}
	}

	public ComposeStateUpdateMessageSystem(Contexts contexts) {

		this.contexts = contexts;
		networkableContexts = contexts.GetNetworkableContexts();

		messageTargets = GetMessageTargets();
	}

	public void Execute() {

		var changes = GetMessageChanges();
		if (changes.Length == 0) return;

		var message = new StateUpdateMessage(
			timestamp: contexts.game.currentTick.value,
			changes: changes
		);

		messageTargets.GetEntities().Each(e => e.EnqueueOutgoingMessage(message));
	}

	IGroup<NetworkingEntity> GetMessageTargets() {

		bool isClientOrServer = (ProgramInstance.thisInstanceKind & InstanceKind.Networked) != 0;
		if (!isClientOrServer) return null;

		var matcher = (ProgramInstance.thisInstanceKind == InstanceKind.Server) ?
			NetworkingMatcher.AllOf(NetworkingMatcher.Client, NetworkingMatcher.OutgoingMessages) :
			NetworkingMatcher.AllOf(NetworkingMatcher.Server, NetworkingMatcher.OutgoingMessages);
		
		return contexts.networking.GetGroup(matcher);
	}

	EntityChange[] GetMessageChanges() {

		List<EntityChange> changes = new List<EntityChange>();

		//int numInBacklog = destroyedEntityChangesBacklog.Count;
		WriteBacklog(changes);
		//UnityEngine.Debug.LogFormat("Processed {0} destroyed entities in backlog. Took {1} bytes.", numInBacklog, writer.BaseStream.Position);

		var entities = entitiesToTrack.ToArray();
		Array.Sort(entities, byPriorityComparer);

		for (int i = 0; i < entities.Length; ++i) {

			var e = entities[i];

			var change = MakeChangeOf(e);
			e.UnsetChangeFlags();
			ResetPriorityOf(e);

			changes.Add(change);

			if (i > maxNumEntityChangesPerMessage) {

				AddToBacklog(entities, startingIndex: i);
				return changes.ToArray();
			}
		}

		return changes.ToArray();
	}

	void WriteBacklog(List<EntityChange> changes) {

		changes.AddRange(destroyedEntityChangesBacklog);
		destroyedEntityChangesBacklog.Clear();
	}

	/// Adds all Entities getting destroyed to the backlog.
	void AddToBacklog(INetworkableEntity[] entities, int startingIndex) {

		for (int i = startingIndex; i < entities.Length; ++i) {

			var e = entities[i];
			if (e.flagDestroy) {
				
				var contextName = e.contextInfo.name;
				int contextIndex = contexts.contextNameToIndex[contextName];
				destroyedEntityChangesBacklog.Add(EntityChange.MakeRemoval(contextIndex, e.id.value));
			}
		}
	}

	EntityChange MakeChangeOf(INetworkableEntity e) {

		var contextName = e.contextInfo.name;
		int contextIndex = contexts.contextNameToIndex[contextName];

		if (e.flagDestroy) {
			return EntityChange.MakeRemoval(contextIndex, e.id.value);
		}

		var componentChanges = MakeComponentChangesOf(e); 
		return EntityChange.MakeUpdate(contextIndex, e.id.value, componentChanges);
	}

	ComponentChange[] MakeComponentChangesOf(INetworkableEntity e) {

		var componentChanges = new List<ComponentChange>();

		var changed = e.changeFlags.flags;
		int numComponents = e.totalComponents;

		for (int i = 0; i < numComponents; ++i) {

			if (e.HasComponent(i)) {
				componentChanges.Add(ComponentChange.MakeUpdate(i, e.GetComponent(i)));
			} else if (changed[i]) {
				componentChanges.Add(ComponentChange.MakeRemoval(i));
			}
		}

		return componentChanges.ToArray();
	}

	/// Sets the accumulated priority of the given Entity to zero.
	void ResetPriorityOf(INetworkableEntity e) {

		if (e.hasNetworkUpdatePriority) {

			e.ReplaceNetworkUpdatePriority(
				newBasePriority: e.networkUpdatePriority.basePriority, 
				newAccumulated: 0
			);
		}
	}
}

/// A NetworkUpdatePriority-based comparer of Entities.
/// For sorting in descending order (highest priority first).
/// Entities which are getting destroyed (e.flagDestroy == true) are automatically highest priority.
public class EntityByPriorityComparer<TEntity> : IComparer<TEntity> where TEntity : IEntity, IDestroy, INetworkUpdatePriority {

	int IComparer<TEntity>.Compare(TEntity a, TEntity b) {

		if (a.flagDestroy && !b.flagDestroy) return -1;
		if (!a.flagDestroy && b.flagDestroy) return 1;

		int priorityA = GetPriorityOf(a);
		int priorityB = GetPriorityOf(b);

		if (priorityA >  priorityB) return -1;
		if (priorityA == priorityB) return  0;
		return 1;
	}

	int GetPriorityOf(TEntity e){

		return e.hasNetworkUpdatePriority ? e.networkUpdatePriority.accumulated : 0;
	}
}