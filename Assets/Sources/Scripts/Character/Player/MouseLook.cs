using System;
using UnityEngine;

/// A mouse look script roughly based on UnityStandardAssets.Characters.FirstPerson.MouseLook.
/// Based on mouse move input found in CharacterInput, rotates the camera (up-down) and the character (left-right).
[Serializable]
public class MouseLook : MonoBehaviour, ICharacterBehaviour {
	
	public Vector2 sensitivity = new Vector2(2f, 2f);

	public bool isRotationSmooth;
	public float smoothingFactor = 5f;

	public bool clampVerticalRotation = true;
	public RangeFloat verticalRotationRange = new RangeFloat(-90f, 90f);

	[SerializeField] Transform characterTransform;
	[SerializeField] Transform cameraTransform;

	Quaternion characterTargetRotation;
	Quaternion cameraTargetRotation;

	public void SimulateStep(PlayerInputState inputState) {

		HandleMouseMove(inputState.mouseMoveAxes);
	}

	void Start() {

		characterTransform = characterTransform ?? transform;
		Debug.Assert(characterTransform != null);

		cameraTransform = cameraTransform ?? GetComponentInChildren<Camera>().transform;
		Debug.Assert(cameraTransform != null);

		InitTargetRotations();
	}

	void InitTargetRotations() {

		characterTargetRotation = characterTransform.localRotation;
		cameraTargetRotation    = cameraTransform.localRotation;
	}

	void HandleMouseMove(Vector2 mouseMoveAxes) {

		var mouseRot = new Vector2(
			mouseMoveAxes.y * sensitivity.y,
			mouseMoveAxes.x * sensitivity.x
		);

		characterTargetRotation *= Quaternion.Euler(0f, mouseRot.y, 0f);
		cameraTargetRotation    *= Quaternion.Euler(-mouseRot.x, 0f, 0f);

		if (clampVerticalRotation) {
			cameraTargetRotation = ClampRotationAroundXAxis(cameraTargetRotation);
		}

		if (isRotationSmooth) {
			
			characterTransform.localRotation = GetSmoothCharacterRotation();
			cameraTransform.localRotation    = GetSmoothCharacterRotation();
			
		} else {
			
			characterTransform.localRotation = characterTargetRotation;
			cameraTransform.localRotation    = cameraTargetRotation;
		}
	}

	Quaternion ClampRotationAroundXAxis(Quaternion q) {
		
		q.x /= q.w;
		q.y /= q.w;
		q.z /= q.w;
		q.w = 1.0f;

		float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

		angleX = verticalRotationRange.Clamp(angleX);

		q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

		return q;
	}

	Quaternion GetSmoothCharacterRotation() {
		
		return Quaternion.Slerp(
			characterTransform.localRotation, 
			characterTargetRotation, 
			smoothingFactor * Time.fixedDeltaTime
		);
	}

	Quaternion GetNextSmoothCameraRotation() {

		return Quaternion.Slerp(
			cameraTransform.localRotation, 
			cameraTargetRotation,
			smoothingFactor * Time.fixedDeltaTime
		);
	}
}
