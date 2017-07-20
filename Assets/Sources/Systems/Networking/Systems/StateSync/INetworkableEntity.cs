using Entitas;
using System.Linq;

/// An entity whose state can be synced across the network.
public interface INetworkableEntity : IEntity, IId, IChangeFlags, INetworkUpdatePriority, IDestroy {}
public partial class GameEntity : INetworkableEntity {}
public partial class InputEntity : INetworkableEntity {}

public static class ContextsExtensionsINetworkableEntity {

	public static IContext<INetworkableEntity>[] GetNetworkableContexts(this Contexts contexts) {

		return contexts.allContexts
			.OfType<IContext<INetworkableEntity>>()
			.ToArray();
	}
}
