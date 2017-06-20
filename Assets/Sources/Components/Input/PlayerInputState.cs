using UnityEngine;

/// Stores all the relevant inputs a player did during a particular simstep. 
public struct PlayerInputState {

	public Vector2 moveAxes;
	public Vector2 mouseMoveAxes;

	// Could replace this with a bit set as the number of possible buttons grows.
	public bool buttonPressedJump;
	public bool buttonPressedFire;
}
