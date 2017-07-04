using System;
using System.Linq;
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

/// Same as Entitas.Systems, but only adds a given system if it is available with the current program instance kind.
/// Checks the SystemAvailability of a given system.
[SystemAvailability(InstanceKind.All)]
public class MySystems : Systems {

	public bool throwExceptionWhenNoAvailabilityAttribute = false;
	public bool logMessageWhenNoAvailabilityAttribute = false;

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

			if (throwExceptionWhenNoAvailabilityAttribute) {

				throw new Exception(String.Format("System {0} has no SystemAvailabilityAttribute!", system));
			} 

			if (logMessageWhenNoAvailabilityAttribute) {
				Debug.LogWarningFormat("System {0} has no SystemAvailabilityAttribute. Defaulting to InstanceKind.All", system);
			}

			return InstanceKind.All;
		}

		var availabilityAttribute = (SystemAvailabilityAttribute)attributes.Last();
		return availabilityAttribute.programInstanceKind;
	}
}
