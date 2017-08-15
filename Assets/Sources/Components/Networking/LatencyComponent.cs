using System;
using Entitas;
using UnityEngine;

/// Stores the amount of time in milliseconds needed to send a message to 
/// or receive a message from the connection its entity represents.
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
