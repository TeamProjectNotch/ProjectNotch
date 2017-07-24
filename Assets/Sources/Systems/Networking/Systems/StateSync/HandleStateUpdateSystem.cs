using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Entitas;
using UnityEngine;

[SystemAvailability(InstanceKind.Client)]
public class HandleStateUpdateSystem : HandleMessageSystem<StateUpdateMessage>, IInitializeSystem {

	readonly Contexts contexts;

	IEntityChangeProcessor[] processors;

	public HandleStateUpdateSystem(Contexts contexts) : base(contexts.networking) {

		this.contexts = contexts;
	}

	public void Initialize() {

		var allContexts = contexts.allContexts;
		int numContexts = allContexts.Length;
		processors = new IEntityChangeProcessor[numContexts];
		for (int contextIndex = 0; contextIndex < numContexts; ++contextIndex) {

			var context = allContexts[contextIndex];
			if (context.IsNetworkable()) {
				
				processors[contextIndex] = MakeChangeProcessor(context);
			}
		}
	}

	protected override IGroup<NetworkingEntity> GetMessageSources(IContext<NetworkingEntity> context) {

		var matcher = (ProgramInstance.thisInstanceKind == InstanceKind.Server) ?
			NetworkingMatcher.AllOf(NetworkingMatcher.Client, NetworkingMatcher.IncomingMessages) :
			NetworkingMatcher.AllOf(NetworkingMatcher.Server, NetworkingMatcher.IncomingMessages);

		return context.GetGroup(matcher);
	}

	protected override void Process(StateUpdateMessage message, NetworkingEntity source) {

		/*var game = contexts.game;
		Debug.LogFormat("Received state update from tick {0}, current tick {1}", 
			message.timestamp, 
			game.hasCurrentTick ? game.currentTick.value.ToString() : (-1).ToString()
		);*/

		message.changes.Each(Process);
		//Debug.LogFormat("Changes applied this step: {0}", message.changes.Length);
	}

	IEntityChangeProcessor MakeChangeProcessor(IContext context) {
		
		var entityType = context.GetEntityType();
		var processorType = typeof(EntityChangeProcessor<>).MakeGenericType(entityType);
		return (IEntityChangeProcessor)Activator.CreateInstance(processorType, context);
	}

	void Process(EntityChange change) {

		var contextIndex = change.contextIndex;
		var processor = processors[change.contextIndex];
		if (processor == null) {
			throw new NullReferenceException(String.Format("Can't find entity change processor for contextIndex {0}", contextIndex));
		}

		processor.Process(change);
	}

	interface IEntityChangeProcessor {

		void Process(EntityChange change);
	}
	 
	/// Processes entity changes in one context.
	class EntityChangeProcessor<TEntity> : IEntityChangeProcessor where TEntity : class, IEntity {

		readonly IContext<TEntity> context;
		readonly PrimaryEntityIndex<TEntity, ulong> entityByIdIndex;

		public EntityChangeProcessor(IContext<TEntity> context) {

			this.context = context;
			entityByIdIndex = (PrimaryEntityIndex<TEntity, ulong>)context.GetEntityIndex(Contexts.Id);
		}

		public void Process(EntityChange change) {

			var e = entityByIdIndex.GetEntity(change.entityId);
			if (e == null) {

				if (change.isRemoval) {

					UnityEngine.Debug.Log("Can't apply an EntityChange (entity removal), since it's Entity doesn't exist.");
					return;
				}

				//Debug.LogFormat("Entity with id {0} not found. Creating...", change.entityId);
				e = context.CreateEntity();
			}

			change.Apply(e);
		}
	}
}
