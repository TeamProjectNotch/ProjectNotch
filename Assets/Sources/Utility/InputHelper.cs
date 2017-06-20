using System;
using UnityEngine;

/// Helper functions atop UnityEngine.Input
public static class InputHelper {

	/// Both horizontal (x) and vertical (y) axes in one vector.
	public static Vector2 GetMoveAxes() {
		
		return new Vector2(
			Input.GetAxis("Horizontal"), 
			Input.GetAxis("Vertical")
		);
	}
		
	public static Vector2 GetMouseMoveAxes() {

		return new Vector2(
			Input.GetAxis("Mouse X"), 
			Input.GetAxis("Mouse Y")
		);
	}
}

