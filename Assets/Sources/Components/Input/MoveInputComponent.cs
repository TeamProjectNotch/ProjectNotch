using System;
using Entitas;
using UnityEngine;

/// Contains the input from the WASD/arrow keys or joystick.
/// So this.axes.x has the same value as Input.GetAxis("Horizontal").
[Input]
public class MoveInputComponent : IComponent, IUnifiedSerializable {

	public Vector2 axes;

	public void Serialize<T>(T s) where T : IUnifiedSerializer {

		s.Serialize(ref axes.x);
		s.Serialize(ref axes.y);
	}
}

