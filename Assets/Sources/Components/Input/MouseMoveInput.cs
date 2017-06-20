using System;
using Entitas;
using UnityEngine;

/// Stores the movement of the mouse this frame.
/// Stores the same values as you would get from 
/// UnityEngine.Input.GetAxis("Mouse X") and ...GetAxis("Mouse Y").
[Input]
public class MouseMoveInput : IComponent, IUnifiedSerializable {
	
	public Vector2 axes;

	public void Serialize<T>(T s) where T : IUnifiedSerializer {

		s.Serialize(ref axes.x);
		s.Serialize(ref axes.y);
	}
}
