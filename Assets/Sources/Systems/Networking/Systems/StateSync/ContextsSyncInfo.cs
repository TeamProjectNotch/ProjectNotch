using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

/// Contains info on which components should be network-synced and which shouldn't.
public static class ContextSyncInfo {

    /// shouldSyncComponent[contextIndex][componentIndex] tells 
    /// if a component of given index in a context of given index should be synced via the network.
    public static readonly bool[][] shouldSyncComponent;

	static ContextSyncInfo() {

		shouldSyncComponent = MakeSyncMap();

        Debug.Log($"ContextSyncInfo: generated component sync map:\n{GetDescription()}");
	}

	static bool[][] MakeSyncMap() {

		return Contexts.sharedInstance.allContexts
			.Select(GetShouldSyncContext)
			.ToArray();
	}

    // TODO Make a new sync map class instead of a bool[][] 
    // and override its ToString
    static string GetDescription() {

        var allContexts = Contexts.sharedInstance.allContexts;
        var builder = new StringBuilder();
        var numContexts = allContexts.Length;
        for (int i = 0; i < numContexts; ++i) {

            var contextInfo = allContexts[i].contextInfo;
            builder.Append(contextInfo.name);
            builder.Append(" (");

            for (int j = 0; j < contextInfo.componentNames.Length; ++j) {

                builder.Append(contextInfo.componentNames[j]);
                builder.Append(" ");
                builder.Append(shouldSyncComponent[i][j]);
                builder.Append(", ");
            }

            builder.AppendLine(")");
        }

        return builder.ToString();
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

        var networkTargets = GetNetworkTargetsOf(componentType);

        if (!ImplementsIUnifiedSerializable(componentType) &&
            networkTargets != NetworkTargets.None) {

			Debug.LogWarning(
				$"Component type {componentType} doesn't implement " +
				"IUnifiedSerializable, but isn't marked as " +
				"non-network-syncable using NetworkSyncAttribute. Is it " +
				"meant to be synced over the network but its " +
				"serialization hasn't been implemented yet?"
			);
			return false;
		}

        bool toClient = (networkTargets & NetworkTargets.Client) != 0;
        bool toServer = (networkTargets & NetworkTargets.Server) != 0;
        return
            (ProgramInstance.isServer && toClient) || 
            (ProgramInstance.isClient && toServer);
	}

    static NetworkTargets GetNetworkTargetsOf(Type componentType) {
        
        var attribute = componentType.GetCustomAttributes(
            typeof(NetworkSyncAttribute),
            inherit: true
        ).LastOrDefault() as NetworkSyncAttribute;

        return attribute?.targets ?? NetworkTargets.Client;
    }

    static bool ImplementsIUnifiedSerializable(Type componentType) {

        return typeof(IUnifiedSerializable).IsAssignableFrom(componentType);
    }
}

