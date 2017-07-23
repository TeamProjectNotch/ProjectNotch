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
}
