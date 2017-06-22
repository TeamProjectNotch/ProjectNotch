using System;
using UnityEngine;

/// Stores the inputs of a character. 
/// Those are read by character-controlling scripts that handle motion and whatever.
/// Exists so that input for those scripts can come from anywhere, and not just UnityEngine.Input.
public class CharacterInput : MonoBehaviour {

	public Vector2 moveAxes      {get; set;}
	public Vector2 mouseMoveAxes {get; set;}

	public bool isJump {get; set;}

	public void Reset() {

		moveAxes = Vector2.zero;
		mouseMoveAxes = Vector2.zero;

		isJump = false;
	}
}
