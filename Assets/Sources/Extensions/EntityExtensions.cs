using System;
using Entitas;

public static class EntityExtensions {
	
	public static void UnsetChangeFlags(this IChangeFlags e) {

		if (!e.hasChangeFlags) {

			throw new InvalidOperationException("Can't unset change flags bacause the given entity doesn't have them!");
		}

		var flags = e.changeFlags.flags;
		flags.Fill(false);
		e.ReplaceChangeFlags(flags);
	}

	/// Sets the accumulated network update priority of the given entity to zero.
	public static void ResetNetworkPriority(this INetworkUpdatePriority e) {

		if (!e.hasNetworkUpdatePriority) {

			throw new InvalidOperationException("Can't reset network update priority bacause the given entity doesn't have it!");
		}

		e.ReplaceNetworkUpdatePriority(
			newBasePriority: e.networkUpdatePriority.basePriority, 
			newAccumulated: 0
		);
	}
}
