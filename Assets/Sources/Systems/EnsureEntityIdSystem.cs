using System;
using System.Linq;
using Entitas;

/// Ensures that all GameEntities have an IdComponent.
[SystemAvailability(InstanceKind.Server | InstanceKind.Singleplayer)]
public class EnsureGameEntityIdSystem : EnsureEntityIdSystem<GameEntity> {

	public EnsureGameEntityIdSystem(Contexts contexts) : base(contexts.game) {}
}

/// Ensures that all InputEntities have an IdComponent.
[SystemAvailability(InstanceKind.Client | InstanceKind.Singleplayer)]
public class EnsureInputEntityIdSystem : EnsureEntityIdSystem<InputEntity> {

	public EnsureInputEntityIdSystem(Contexts contexts) : base(contexts.input) {}
}

/// An abstract System which ensures that all Entities of given type have an IdComponent, 
/// provided a Context of those Entities.
public abstract class EnsureEntityIdSystem<TEntity> : IInitializeSystem
	where TEntity : class, IEntity, IId  {

	readonly IContext<TEntity> context;
	readonly int idComponentIndex;

	ulong nextId = 0;

	protected EnsureEntityIdSystem(IContext<TEntity> context) {

		this.context = context;

		idComponentIndex = FindIdComponentIndex(context);
	}

	public void Initialize() {

		SetNextIdBasedOnExistingEntities(); 
		EnsureIn(context);
	}

	int FindIdComponentIndex(IContext<TEntity> context) {

		var types = context.contextInfo.componentTypes;
		var idComponentType = typeof(IdComponent);
		var index = types.IndexOf(idComponentType);

		if (index == -1) {

			throw new Exception(String.Format(
				"The context {0} does not have a component {1}", 
				context,
				idComponentType
			));
		}

		return index;
	}

	// This lets the system successfully initialize even 
	// if there are already entities in the context. E.g. after loading a save file.
	void SetNextIdBasedOnExistingEntities() {

		var entitiesWithId = context.GetEntities(Matcher<TEntity>.AllOf(idComponentIndex));
		nextId = entitiesWithId.Any() ? entitiesWithId.Max(e => GetId(e)) + 1 : 0;
	}

	ulong GetId(TEntity entity) {

		var idComponent = (IdComponent)entity.GetComponent(idComponentIndex);
		return idComponent.value;
	}

	void EnsureIn(IContext<TEntity> context) {

		context.GetEntities().Each(AssignId);
		context.OnEntityCreated += (c, entity) => AssignId(entity);
	}

	void AssignId(IEntity entity) {

		var e = (IId)entity;
		e.AddId(nextId);
		nextId += 1;
	}
}
