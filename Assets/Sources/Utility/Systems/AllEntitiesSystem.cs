using System;
using System.Linq;
using Entitas;

/// Applies an action to all entities of type TEntity in all contexts. TEntity can be an interface.
public abstract class AllEntitiesSystem<TEntity> : IInitializeSystem {

	readonly Contexts contexts;

	public AllEntitiesSystem(Contexts contexts) {

		this.contexts = contexts;
	}

	public virtual void Initialize() {

		contexts.allContexts
			.Where(IsValid)
			.Select(context => MakeExecutor(context))
			.Each(executor => executor.Execute(Execute));
	}

	bool IsValid(IContext context) {

		return typeof(TEntity).IsAssignableFrom(context.GetEntityType());
	}

	IAllEntitiesActionExecutor MakeExecutor(IContext context) {

		// We need two types because the executor will actually 
		// be of type AllEntitiesSystem<TEntity>.AllEntitiesActionExecutor<[entity type here]>
		var desiredEntityType = typeof(TEntity);
		var contextEntityType = context.GetEntityType();
		var genericTypeDefinition = typeof(AllEntitiesActionExecutor<>);
		var executorType = genericTypeDefinition.MakeGenericType(
			desiredEntityType, 
			contextEntityType
		);

		return (IAllEntitiesActionExecutor)Activator.CreateInstance(executorType, context);
	}

	protected abstract void Execute(TEntity entity);

	void Execute(IEntity entity) {Execute((TEntity)entity);}

	interface IAllEntitiesActionExecutor {void Execute(Action<IEntity> action);}
	class AllEntitiesActionExecutor<T> : IAllEntitiesActionExecutor where T : class, IEntity {

		readonly IContext<T> context;

		public AllEntitiesActionExecutor(IContext<T> context) {

			this.context = context;
		}

		public void Execute(Action<IEntity> action) {

			// All existing entities...
			context.GetEntities().Each(action);
			// ...And all future ones.
			context.OnEntityCreated += (c, entity) => action(entity);
		}
	} 
}
