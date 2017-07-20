using System;
using System.Linq;
using System.Collections.Generic;
using Entitas;
using System.IO;

[SystemAvailability(InstanceKind.Networked)]
public class ComposeStateUpdateMessageSystem : MultiReactiveSystem<INetworkableEntity, Contexts> {

	const int preferredNumBytesPerMessage = 1024;
	const int maxNumEntityChangesPerMessage = 5;

	readonly Contexts contexts;
	readonly IGroup<NetworkingEntity> networkDestinations;

	readonly EntityByPriorityComparer<INetworkableEntity> byPriorityComparer = new EntityByPriorityComparer<INetworkableEntity>();
	readonly List<ulong> destroyedEntitiesBacklog = new List<ulong>();

	public ComposeStateUpdateMessageSystem(Contexts contexts) : base(contexts) {

		this.contexts = contexts;
		networkDestinations = GetNetworkDestinations();
	}

	IGroup<NetworkingEntity> GetNetworkDestinations() {

		bool isClientOrServer = (ProgramInstance.thisInstanceKind & InstanceKind.Networked) != 0;
		if (!isClientOrServer) return null;

		var matcher = (ProgramInstance.thisInstanceKind == InstanceKind.Server) ?
			NetworkingMatcher.AllOf(NetworkingMatcher.Client, NetworkingMatcher.OutgoingMessages) :
			NetworkingMatcher.AllOf(NetworkingMatcher.Server, NetworkingMatcher.OutgoingMessages);

		return contexts.networking.GetGroup(matcher);
	}

	protected override ICollector[] GetTrigger(Contexts contexts) {

		return new ICollector[]{
			
			contexts.game .CreateCollector(GameMatcher .NetworkUpdatePriority.Added()),
			contexts.input.CreateCollector(InputMatcher.NetworkUpdatePriority.Added()),
		};
	}

	protected override bool Filter(INetworkableEntity entity) {
		
		return entity.hasChangeFlags;
	}

	protected override void Execute(List<INetworkableEntity> entities) {

		var message = new GameStateUpdateMessage(
			timestamp: contexts.game.currentTick.value,
			changes: GetMessageChanges(entities)
		);

		networkDestinations.GetEntities().Each(e => e.EnqueueOutgoingMessage(message));
	}

	EntityChange[] GetMessageChanges(List<INetworkableEntity> entitiesToTrack) {

		List<EntityChange> changes = new List<EntityChange>();

		//int numInBacklog = destroyedEntitiesBacklog.Count;
		WriteBacklog(changes);
		//UnityEngine.Debug.LogFormat("Processed {0} destroyed entities in backlog. Took {1} bytes.", numInBacklog, writer.BaseStream.Position);

		var entities = entitiesToTrack.ToArray();
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
	void AddToBacklog(INetworkableEntity[] entities, int startingIndex) {

		for (int i = startingIndex; i < entities.Length; ++i) {

			var e = entities[i];
			if (e.flagDestroy) {

				destroyedEntitiesBacklog.Add(e.id.value);
			}
		}
	}

	EntityChange MakeChangeOf(INetworkableEntity e) {

		if (e.flagDestroy) {
			return EntityChange.MakeRemoval(e.id.value);
		}

		var componentChanges = MakeComponentChangesOf(e); 
		return EntityChange.MakeUpdate(e.id.value, componentChanges);
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

	void UnsetChangeFlagsOf(INetworkableEntity e) {

		var flags = e.changeFlags.flags;
		flags.Fill(false);
		e.ReplaceChangeFlags(flags);
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

/// Composes state update messages for a given context.
/// The entities in the context must have an Id.
/// The following components are required to be defined in the context:
/// Id, ChangeFlags, NetworkUpdatePriority, Destroy. 
[SystemAvailability(InstanceKind.Networked)]
public class ComposeContextStateUpdateMessageSystem<TEntity> : IExecuteSystem 
	where TEntity : class, IEntity, IId, IChangeFlags, INetworkUpdatePriority, IDestroy {

	const int preferredNumBytesPerMessage = 1024;
	const int maxNumEntityChangesPerMessage = 5;

	readonly NetworkingContext networking;
	readonly GameContext game;

	readonly IGroup<NetworkingEntity> clients;
	readonly IGroup<TEntity> entitiesToTrack;

	readonly EntityByPriorityComparer<TEntity> byPriorityComparer = new EntityByPriorityComparer<TEntity>();
	readonly List<ulong> destroyedEntitiesBacklog = new List<ulong>();

	public ComposeContextStateUpdateMessageSystem(Contexts contexts, IContext<TEntity> context) {

		networking = contexts.networking;
		game = contexts.game;

		var componentIndex = context.FindIndexOfComponent<ChangeFlagsComponent>();
		var matcher = Matcher<TEntity>.AllOf(componentIndex);
		entitiesToTrack = context.GetGroup(matcher);

		clients = networking.GetGroup(
			NetworkingMatcher.AllOf(NetworkingMatcher.Client, NetworkingMatcher.OutgoingMessages)
		);
	}

	public void Execute() {

		var message = MakeMessage();

		foreach (var e in clients.GetEntities()) {

			e.EnqueueOutgoingMessage(message);
		}
	}

	INetworkMessage MakeMessage() {

		return new GameStateUpdateMessage(
			timestamp: game.currentTick.value,
			changes: GetMessageChanges()
		);
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
	void AddToBacklog(TEntity[] entities, int startingIndex) {

		for (int i = startingIndex; i < entities.Length; ++i) {

			var e = entities[i];
			if (e.flagDestroy) {

				destroyedEntitiesBacklog.Add(e.id.value);
			}
		}
	}

	EntityChange MakeChangeOf(TEntity e) {

		if (e.flagDestroy) {
			return EntityChange.MakeRemoval(e.id.value);
		}

		var componentChanges = MakeComponentChangesOf(e); 
		return EntityChange.MakeUpdate(e.id.value, componentChanges);
	}

	ComponentChange[] MakeComponentChangesOf(TEntity e) {

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

	void UnsetChangeFlagsOf(TEntity e) {

		var flags = e.changeFlags.flags;
		flags.Fill(false);
		e.ReplaceChangeFlags(flags);
	}

	/// Sets the accumulated priority of the given Entity to zero.
	void ResetPriorityOf(TEntity e) {

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
public class EntityByPriorityComparer<TEntity> : IComparer<TEntity> where TEntity : IEntity, INetworkUpdatePriority {

	int IComparer<TEntity>.Compare(TEntity a, TEntity b) {

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