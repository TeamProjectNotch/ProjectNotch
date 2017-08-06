using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Entitas;
using UnityEngine;

[SystemAvailability(InstanceKind.Networked)]
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
        var parameters = new object[] {contexts, context};
		return (IEntityChangeProcessor)Activator.CreateInstance(processorType, parameters);
	}

	void Process(EntityChange change) {

		var contextIndex = change.contextIndex;
		var processor = processors[contextIndex];
		if (processor == null) {
			throw new NullReferenceException(String.Format("Can't find entity change processor for contextIndex {0}", contextIndex));
		}

		processor.Process(change);
	}

	interface IEntityChangeProcessor {

		void Process(EntityChange change);
	}
	 
	/// Processes entity changes in one context.
	class EntityChangeProcessor<TEntity> : IEntityChangeProcessor 
        where TEntity : class, IEntity, INetworkableEntity {

        readonly GameContext game;
		readonly IContext<TEntity> context;
		readonly PrimaryEntityIndex<TEntity, ulong> entityByIdIndex;

		public EntityChangeProcessor(Contexts contexts, IContext<TEntity> context) {

            game = contexts.game;
			this.context = context;

			entityByIdIndex = (PrimaryEntityIndex<TEntity, ulong>)context.GetEntityIndex(Contexts.Id);
		}

		public void Process(EntityChange change) {
            
			var e = GetEntityFor(change);
            if (e == null) return;

            change.Apply(e);
            UpdateNextId(e);
        }

        TEntity GetEntityFor(EntityChange change) {

            var e = entityByIdIndex.GetEntity(change.entityId);
            if (e == null) {

                if (change.isRemoval) {

                    Debug.Log("Can't apply an EntityChange (entity removal), since it's Entity doesn't exist.");
                    return null;
                }

                //Debug.LogFormat("Entity with id {0} not found. Creating...", change.entityId);
                e = context.CreateEntity();
            }

            return e;
        }

        void UpdateNextId(TEntity e) {

            if (e.hasId) {

                ulong id = e.id.value;
                if (id >= game.nextId.value) {

                    game.ReplaceNextId(id + 1ul);
                }
            }
        }
	}
}
