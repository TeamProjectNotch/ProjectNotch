using System;
using Entitas;
using System.Linq;

/// An entity whose state can be synced across the network.
public interface INetworkableEntity : IEntity, IId, IChangeFlags, INetworkUpdatePriority, IDestroy {}
public partial class GameEntity : INetworkableEntity {}
public partial class InputEntity : INetworkableEntity {}

public static class ContextsExtensionsINetworkableEntity {

	static readonly Type networkableEntityType = typeof(INetworkableEntity);

	public static IContext[] GetNetworkableContexts(this Contexts contexts) {

		return contexts.allContexts
			.Where(context => context.IsNetworkable())
			.ToArray();
	}

	/// Do entities in this context implement INetworkableEntity?
	public static bool IsNetworkable(this IContext context) {

		var entityType = context.GetEntityType();
		//UnityEngine.Debug.LogFormat("{0} is IContext<{1}>", context, entityType);
		return networkableEntityType.IsAssignableFrom(entityType);
	}

	/// GameContext -> GameEntity, InputContext -> InputEntity etc.
	public static Type GetEntityType(this IContext context) {

		// IContext<GameEntity>, IContext<InputEntity> etc.
		var interfaceType = context
			.GetType()
			.GetInterfaces()
			.Where(type => type.IsGenericType)
			.Where(type => type.GetGenericTypeDefinition() == typeof(IContext<>))
			.SingleOrDefault();

		if (interfaceType == null) {
			throw new ArgumentException(String.Format("Given context {0} does not implement IContext<>!", context));
		}

		// IContext<GameEntity> -> GameEntity
		// IContext<InputEntity> -> InputEntity
		// IA<B> -> B
		return interfaceType.GetGenericArguments()[0];
	}
}
