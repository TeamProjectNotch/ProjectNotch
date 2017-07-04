using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

/// Handles the bob of a handheld item when the character holding it walks.
[System.Serializable]
public class HandheldItemBobManager : MonoBehaviour, ICharacterBehaviour {

	public CharacterController characterController;
	public Transform handleTransform;

	public Vector2 bobRange = new Vector2(0.0015f, 0.0025f);
	public float kickStrength = 1000f;

	Vector2 bobOffset;
	float recoil;

	Vector3 defaultHandlePosition;
	Quaternion defaultHandleRotation;

	PlayerInputState inputState;

	void Start () {

		characterController = characterController ?? GetComponentInParent<CharacterController>();
		Assert.IsNotNull(characterController);

		defaultHandlePosition = handleTransform.localPosition - new Vector3(0, 0.05f, 0);
		defaultHandleRotation = handleTransform.rotation;

		StartCoroutine(bobX());
		StartCoroutine(bobY());
	}

	public void SimulateStep(PlayerInputState inputState) {

		this.inputState = inputState;

		if (inputState.buttonPressedFire) {
			StartCoroutine(kick());
		}

		if (inputState.moveAxes == Vector2.zero) {
			bobOffset = Vector2.zero;
		}

		var mouseMove = inputState.mouseMoveAxes * Time.deltaTime;

		var newHandlePosition = defaultHandlePosition 
			+ (Vector3)mouseMove
			- (Vector3)inputState.moveAxes / 20f
			- new Vector3(0, characterController.velocity.y / 150);

		handleTransform.localPosition = Vector3.Lerp(
			handleTransform.localPosition + new Vector3(bobOffset.x, -bobOffset.y, recoil * 1.5f),
			newHandlePosition, 
			Time.deltaTime * 5
		);

		handleTransform.localRotation = Quaternion.Lerp(
			handleTransform.localRotation, 
			defaultHandleRotation * Quaternion.AngleAxis(Random.Range(-recoil, recoil) * kickStrength, Vector3.up), 
			Time.deltaTime * 5
		);
	}

	IEnumerator kick() {

		while (inputState.buttonPressedFire) {

			recoil = -0.02f;
			yield return new WaitForSeconds(0.05f);
			recoil = 0;
			yield return new WaitForSeconds(0.1f);
		}

		recoil = 0;
	}

	IEnumerator bobX() {

		while (enabled) {

			bobOffset.x = bobRange.x;
			yield return new WaitForSeconds(0.3f);
			bobOffset.x = -bobRange.x;
			yield return new WaitForSeconds(0.3f);
		}
	}

	IEnumerator bobY() {

		while (enabled) {

			bobOffset.y = bobRange.y;
			yield return new WaitForSeconds(0.15f);
			bobOffset.y = -bobRange.y;
			yield return new WaitForSeconds(0.15f);
		}
	}
}
