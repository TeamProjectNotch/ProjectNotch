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
			.Select(context => MakeApplier(context))
			.Each(applier => applier.Apply(Apply));
	}

	bool IsValid(IContext context) {

		return typeof(TEntity).IsAssignableFrom(context.GetEntityType());
	}

	IAllEntitiesActionApplier MakeApplier(IContext context) {

		// We need two types because the executor will actually 
		// be of type AllEntitiesSystem<TEntity>.AllEntitiesActionApplier<[entity type here]>
		var desiredEntityType = typeof(TEntity);
		var contextEntityType = context.GetEntityType();
		var genericTypeDefinition = typeof(AllEntitiesActionApplier<>);
		var applierType = genericTypeDefinition.MakeGenericType(
			desiredEntityType, 
			contextEntityType
		);

		return (IAllEntitiesActionApplier)Activator.CreateInstance(applierType, context);
	}

	protected abstract void Apply(TEntity entity);

	void Apply(IEntity entity) {Apply((TEntity)entity);}

	interface IAllEntitiesActionApplier {void Apply(Action<IEntity> action);}
	class AllEntitiesActionApplier<T> : IAllEntitiesActionApplier where T : class, IEntity {

		readonly IContext<T> context;

		public AllEntitiesActionApplier(IContext<T> context) {

			this.context = context;
		}

		public void Apply(Action<IEntity> action) {

			// All existing entities...
			context.GetEntities().Each(action);
			// ...And all future ones.
			context.OnEntityCreated += (c, entity) => action(entity);
		}
	} 
}
