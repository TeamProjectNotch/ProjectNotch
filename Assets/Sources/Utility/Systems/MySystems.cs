using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entitas;

[AttributeUsage(AttributeTargets.Class)]
public class SystemAvailabilityAttribute : Attribute {

	public InstanceKind programInstanceKind;

	public SystemAvailabilityAttribute(InstanceKind programInstanceKind) {

		this.programInstanceKind = programInstanceKind;
	}
}

/// The different kinds of thing the program instance can be. E.G. server, client etc.
public enum InstanceKind {

	None = 0,
	
	Server = 1,
	Client = 2,
	Singleplayer = 4,

	All = 7
}

[SystemAvailability(InstanceKind.Server | InstanceKind.Client | InstanceKind.Singleplayer)]
public class MySystems : Systems {

	const bool tolerateNoAvailabilityAttribute = true;

	InstanceKind programInstanceKind;

	public MySystems(Contexts contexts) : base() {

		programInstanceKind = contexts.gameState.programInstanceKind.value;
	}

	public override Systems Add(ISystem system) {

		var availableInstanceKinds = GetAvailability(system);

		if ((this.programInstanceKind & availableInstanceKinds) == 0) {

			// The system is not available on this program instance kind. Don't add it.
			return this;
		}

		return base.Add(system);
	}

	InstanceKind GetAvailability(ISystem system) {
		
		var attributes = system.GetType().GetCustomAttributes(typeof(SystemAvailabilityAttribute), inherit: true);
		if (attributes.Length == 0) {

			if (tolerateNoAvailabilityAttribute) {

				Debug.LogWarningFormat("System {0} has no SystemAvailabilityAttribute. Defaulting to InstanceKind.All", system);
				return InstanceKind.All;
			} 

			throw new Exception(String.Format("System {0} has no SystemAvailabilityAttribute!", system));
		}

		var availabilityAttribute = (SystemAvailabilityAttribute)attributes[0];
		return availabilityAttribute.programInstanceKind;
	}
}
