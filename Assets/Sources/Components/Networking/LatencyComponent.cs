using System;
using Entitas;
using UnityEngine;

/// Stores the amount of time needed to send a message to or receive from the connection this entity represents.
[Networking]
public class LatencyComponent : IComponent {

	public int ms;

	public float ticks {
		get {
			
			var numTicksPerSecond = 1f / Time.fixedDeltaTime;
			return ms * numTicksPerSecond / 1000f;
		}
	}
}
