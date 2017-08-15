using System;
using System.Collections.Generic;
using System.Linq;
using Entitas;

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

        return context.EntityIs(networkableEntityType);
    }

}

public static class ContextsExtensionsGetNetworkableEntities {

    static Func<IEnumerable<INetworkableEntity>> getter;

    public static IEnumerable<INetworkableEntity> GetNetworkableEntities(this Contexts contexts) {

        return getter();
    }

    static ContextsExtensionsGetNetworkableEntities() {

        var getters = Contexts
            .sharedInstance
            .GetNetworkableContexts()
            .Select(MakeGetterForContext)
            .ToArray();

        getter = () => getters.SelectMany(contextGetter => contextGetter());
    }

    static Func<INetworkableEntity[]> MakeGetterForContext(IContext context) {

        return (Func<INetworkableEntity[]>)Delegate.CreateDelegate(
            typeof(Func<INetworkableEntity[]>), 
            context, "GetEntities"
        );
    }
}