using System;
using System.Linq;
using System.Collections.Generic;
using Entitas;

/// Contains info on which components should be network-synced and which shouldn't.
public static class ContextSyncInfo {

	/// shouldSyncComponent[contextIndex][componentIndex] tells 
	/// if a component of given index in a context of given index should be synced via the network.
	public static bool[][] shouldSyncComponent {get; private set;}

	static ContextSyncInfo() {

		InitializeSyncMap();
	}

	static void InitializeSyncMap() {

		shouldSyncComponent = Contexts.sharedInstance.allContexts
			.Select(GetShouldSyncContext)
			.ToArray();
	}

	static bool[] GetShouldSyncContext(IContext context) {
		
		if (!context.IsNetworkable()) {
			
			return new bool[context.totalComponents];
		}

		return context.contextInfo.componentTypes
			.Select(GetShouldSyncComponent)
			.ToArray();
	}

	static bool GetShouldSyncComponent(Type componentType) {

		var attributes = componentType.GetCustomAttributes(
			typeof(NetworkSyncAttribute), 
			inherit: true
		);

		if (attributes.Any()) {

			var syncInfo = (NetworkSyncAttribute)attributes.Last();
			return
				(ProgramInstance.isServer && syncInfo.toClient) ||
				(ProgramInstance.isClient && syncInfo.toServer);
		} 

		return ProgramInstance.isServer;
	}
}

