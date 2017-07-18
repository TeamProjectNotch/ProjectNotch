using System;
using System.Collections.Generic;
using System.Linq;
using Entitas;

[AttributeUsage(AttributeTargets.Class)]
public class NetworkSyncAttribute : Attribute {

	public bool toClient;
	public bool toServer;

	public NetworkSyncAttribute(bool toClient, bool toServer) {

		this.toClient = toClient;
		this.toServer = toServer;
	}
}

public class UpdateChangeFlagsInContextSystem<TEntity> : IInitializeSystem
	where TEntity : class, IEntity, IChangeFlags {

	readonly bool shouldSyncByDefault = true;
	readonly bool[] shouldSyncComponents;

	readonly IContext<TEntity> context;

	public UpdateChangeFlagsInContextSystem(Contexts contexts, IContext<TEntity> context) {
		
		this.context = context;

		var numComponents = context.totalComponents;
		shouldSyncComponents = new bool[numComponents];
		var componentTypes = context.contextInfo.componentTypes;

		for (int i = 0; i < numComponents; ++i) {

			var attributes = componentTypes[i].GetCustomAttributes(typeof(NetworkSyncAttribute), inherit: true);
			var shouldSync = shouldSyncByDefault;
			if (attributes.Any()) {

				var attribute = (NetworkSyncAttribute)attributes.Last();
				shouldSync =
					((ProgramInstance.thisInstanceKind == InstanceKind.Server) && attribute.toClient) ||
					((ProgramInstance.thisInstanceKind == InstanceKind.Client) && attribute.toServer);
			}

			shouldSyncComponents[i] = shouldSync;
		}
	}

	/// Makes sure that all current *and* future Entities are kept track of.
	public void Initialize() {

		context.GetEntities().Each(SubscribeToEntityEvents);
		context.OnEntityCreated += (c, entity) => SubscribeToEntityEvents(entity);
	}

	/// Makes sure that whenever the given Entity has a component added, replaced, or removed, the changes are recorded.
	void SubscribeToEntityEvents(IEntity e) {

		e.OnComponentAdded += OnComponentAdded;
		e.OnComponentReplaced += OnComponentReplaced;
		e.OnComponentRemoved += OnComponentRemoved;

		context.OnEntityWillBeDestroyed += OnEntityWillBeDestroyed;
	}

	void OnComponentAdded(IEntity e, int componentIndex, IComponent component) {
		SetChangedFlag((TEntity)e, componentIndex);
	}

	void OnComponentReplaced(IEntity e, int componentIndex, IComponent previousComponent, IComponent newComponent) {
		SetChangedFlag((TEntity)e, componentIndex);
	}

	void OnComponentRemoved(IEntity e, int componentIndex, IComponent component) {
		SetChangedFlag((TEntity)e, componentIndex);
	}

	void OnEntityWillBeDestroyed(IContext context, IEntity e) {

		// So that it doesn't get pointlessly called when the entity removes all of its components during destruction.
		e.OnComponentRemoved -= OnComponentRemoved;
	}

	void SetChangedFlag(TEntity entity, int componentIndex) {

		if (!shouldSyncComponents[componentIndex]) return;
		entity.changeFlags.flags[componentIndex] = true;
	}
}