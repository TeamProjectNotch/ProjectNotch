using System;
using UnityEngine;

/// Reads values from UnityEngine.Input into a provided CharacterInput.
[Serializable]
public class CharacterInputReader : MonoBehaviour {

	public CharacterInput target;

	void Start() {

		target = target ?? GetComponent<CharacterInput>();
		Debug.Assert(target != null);
	}

	void Update() {

		target.moveAxes = InputHelper.GetMoveAxes();
		target.mouseMoveAxes = InputHelper.GetMouseMoveAxes();
		target.isJump = Input.GetKeyDown(KeyCode.Space);
	}
}
