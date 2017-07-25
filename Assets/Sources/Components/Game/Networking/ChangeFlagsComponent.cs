using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entitas;

/// For keeping track of Entities whose Components have changed.
/// If flags[index] is true, then the IComponent with that index has been changed (added/removed/replaced).
[Game, Input, Events]
[NetworkSync(NetworkTargets.None)]
public class ChangeFlagsComponent : IComponent {

	public bool[] flags;

	public bool HasAnyFlagsSet {
		get {

			return flags.Count(true) > 0;
		}
	}
}
