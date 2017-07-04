using System;
using Entitas;
using UnityEngine;

/// Stores the amount of time needed to send a message to or receive from the connection this entity represents.
[Networking]
public class LatencyComponent : IComponent {

	public int ms;

	public ulong ticks {
		get {
			
			var numTicksPerSecond = 1f / Time.fixedDeltaTime;
			return (ulong)Math.Ceiling(ms * numTicksPerSecond / 1000f);
		}
	}
}
