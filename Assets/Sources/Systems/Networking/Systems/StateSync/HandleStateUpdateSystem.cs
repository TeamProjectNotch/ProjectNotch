using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Entitas;
using UnityEngine;

using NM = NetworkingMatcher;

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

        var matcher = ProgramInstance.isServer ?
			NM.AllOf(NM.Client, NM.IncomingMessages) :
			NM.AllOf(NM.Server, NM.IncomingMessages);

		return context.GetGroup(matcher);
	}

	protected override void Process(StateUpdateMessage message, NetworkingEntity source) {

		/*var game = contexts.game;
		Debug.LogFormat("Received state update from tick {0}, current tick {1}", 
			message.timestamp, 
			game.hasCurrentTick ? game.currentTick.value.ToString() : (-1).ToString()
		);*/
        
		message.changes.Each(Process);
        Debug.LogFormat(
            "HandleStateUpdateSystem: Applied changes this tick: {0}", 
            message.changes.Length
        );
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
            throw new NullReferenceException(
                "HandleStateUpdateSystem: no processor " +
                $"for contextIndex {contextIndex} " +
                $"(context {contexts.allContexts[contextIndex]})"
            );
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

			entityByIdIndex = 
                (PrimaryEntityIndex<TEntity, ulong>)
                context.GetEntityIndex(Contexts.Id);
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

                    Debug.LogError(
                        "EntityChangeProcessor: " +
                        $"Entity (id: {change.entityId}) not found for removal!"
                    );
                    return null;
                }

                Debug.Log(
                    "EntityChangeProcessor: " +
                    $"Entity (id: {change.entityId}) created" 
                );
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
