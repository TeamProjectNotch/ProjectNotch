﻿using System;
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

		var allContexts = Contexts.sharedInstance.allContexts;

		var numContexts = allContexts.Length;
		shouldSyncComponent = new bool[numContexts][];
		for (int contextIndex = 0; contextIndex < numContexts; ++contextIndex) {

			var context = allContexts[contextIndex];
			var numComponents = context.totalComponents;
			shouldSyncComponent[contextIndex] = new bool[numComponents];
			if (!context.IsNetworkable()) continue;

			var componentTypes = context.contextInfo.componentTypes;
			for (int componentIndex = 0; componentIndex < numComponents; ++componentIndex) {

				var componentType = componentTypes[componentIndex];
				var shouldSync = GetShouldSync(componentType);
				shouldSyncComponent[contextIndex][componentIndex] = shouldSync;
			}
		}
	}

	static IContext[] GetNetworkableContexts() {

		return Contexts.sharedInstance.GetNetworkableContexts();
	}

	static bool GetShouldSync(Type componentType) {

		var attributes = componentType.GetCustomAttributes(
			typeof(NetworkSyncAttribute), 
			inherit: true
		);

		if (attributes.Any()) {

			var syncInfo = (NetworkSyncAttribute)attributes.Last();
			return
				((ProgramInstance.thisInstanceKind == InstanceKind.Server) && syncInfo.toClient) ||
				((ProgramInstance.thisInstanceKind == InstanceKind.Client) && syncInfo.toServer);
		} 

		return (ProgramInstance.thisInstanceKind == InstanceKind.Server);
	}
}

