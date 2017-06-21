using System;
using System.Linq;
using System.Collections.Generic;
using Entitas;
using System.IO;

/// Creates messages with game context changes to send to the clients. 
/// TEMP Currently makes just one message for all clients.
public class ComposeGameChangeMessageSystem : IExecuteSystem {

	const int preferredNumBytesPerMessage = 1024;
	const int maxNumEntityChangesPerMessage = 5;

	readonly NetworkingContext networking;
	readonly GameContext game;

	readonly IGroup<NetworkingEntity> clients;
	readonly IGroup<GameEntity> entitiesToTrack;

	readonly GameEntityByPriorityComparer byPriorityComparer = new GameEntityByPriorityComparer();
	readonly List<ulong> destroyedEntitiesBacklog = new List<ulong>();

	public ComposeGameChangeMessageSystem(Contexts contexts) {

		networking = contexts.networking;
		game = contexts.game;

		entitiesToTrack = contexts.game.GetGroup(GameMatcher.ChangeFlags);
		clients = networking.GetGroup(
			NetworkingMatcher.AllOf(NetworkingMatcher.Client, NetworkingMatcher.OutgoingMessages)
		);
	}

	public void Execute() {
		
		var message = MakeMessage();

		foreach (var e in clients.GetEntities()) {

			var queue = e.outgoingMessages.queue;
			queue.Enqueue(message);
			e.ReplaceOutgoingMessages(queue);
		}
	}

	INetworkMessage MakeMessage() {

		var timestamp = game.currentTick.value;
		var changes = GetMessageChanges();
		return new GameStateUpdateMessage(timestamp, changes);
	}

	EntityChange[] GetMessageChanges() {
		
		List<EntityChange> changes = new List<EntityChange>();

		int numInBacklog = destroyedEntitiesBacklog.Count;
		WriteBacklog(changes);
		//UnityEngine.Debug.LogFormat("Processed {0} destroyed entities in backlog. Took {1} bytes.", numInBacklog, writer.BaseStream.Position);

		var entities = entitiesToTrack.GetEntities();
		Array.Sort(entities, byPriorityComparer);

		for (int i = 0; i < entities.Length; ++i) {

			var e = entities[i];

			var change = MakeChangeOf(e);
			UnsetChangeFlagsOf(e);
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
		
		foreach (var id in destroyedEntitiesBacklog) {
			changes.Add(EntityChange.MakeRemoval(id));
		}

		destroyedEntitiesBacklog.Clear();
	}

	/// Adds all Entities getting destroyed to the backlog.
	void AddToBacklog(GameEntity[] entities, int startingIndex) {

		for (int i = startingIndex; i < entities.Length; ++i) {

			var e = entities[i];
			if (e.flagDestroy) {
				
				destroyedEntitiesBacklog.Add(e.id.value);
			}
		}
	}

	EntityChange MakeChangeOf(GameEntity e) {

		if (e.flagDestroy) {
			return EntityChange.MakeRemoval(e.id.value);
		}

		var componentChanges = MakeComponentChangesOf(e); 
		return EntityChange.MakeUpdate(e.id.value, componentChanges);
	}

	ComponentChange[] MakeComponentChangesOf(GameEntity e) {

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

	void UnsetChangeFlagsOf(GameEntity e) {
		
		var flags = e.changeFlags.flags;
		flags.Fill(false);
		e.ReplaceChangeFlags(flags);
	}

	/// Sets the accumulated priority of the given Entity to zero.
	void ResetPriorityOf(GameEntity e) {

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
public class GameEntityByPriorityComparer : IComparer<GameEntity> {

	int IComparer<GameEntity>.Compare(GameEntity a, GameEntity b) {

		int priorityA = GetPriorityOf(a);
		int priorityB = GetPriorityOf(b);

		if (priorityA >  priorityB) return -1;
		if (priorityA == priorityB) return  0;
		return 1;
	}

	int GetPriorityOf(GameEntity e){

		return e.hasNetworkUpdatePriority ? e.networkUpdatePriority.accumulated : 0;
	}
}
