using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entitas;
using Entitas.VisualDebugging.Unity;

/// Same as Entitas.VisualDebugging.Unity.DebugSystems, but with functionality of MySystems.
[SystemAvailability(InstanceKind.Server | InstanceKind.Client | InstanceKind.Singleplayer)]
public class MyDebugSystems : DebugSystems {

	public bool tolerateNoAvailabilityAttribute = true;
	public bool logMessageWhenNoAvailabilityAttribute = true;

	public MyDebugSystems(string name) : base(name) {}

	public MyDebugSystems(bool noInit) : base(noInit) {}

	public override Systems Add(ISystem system) {

		var availableInstanceKinds = GetAvailability(system);

		if ((ProgramInstance.thisInstanceKind & availableInstanceKinds) == 0) {

			// The system is not available on this program instance kind. Don't add it.
			return this;
		}

		return base.Add(system);
	}

	InstanceKind GetAvailability(ISystem system) {

		var attributes = system.GetType().GetCustomAttributes(typeof(SystemAvailabilityAttribute), inherit: true);
		if (attributes.Length == 0) {

			if (tolerateNoAvailabilityAttribute) {

				if (logMessageWhenNoAvailabilityAttribute) {
					Debug.LogWarningFormat("System {0} has no SystemAvailabilityAttribute. Defaulting to InstanceKind.All", system);
				}

				return InstanceKind.All;
			} 

			throw new Exception(String.Format("System {0} has no SystemAvailabilityAttribute!", system));
		}

		var availabilityAttribute = (SystemAvailabilityAttribute)attributes[0];
		return availabilityAttribute.programInstanceKind;
	}
}

