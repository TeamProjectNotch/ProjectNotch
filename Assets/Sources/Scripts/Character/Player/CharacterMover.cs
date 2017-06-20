using System;
using UnityEngine;

/// Moves the character contoller based on inputs in the provided CharacterInput.
/// **Very** roughly based on UnityStandardAssets.Characters.FirstPerson.FirstPersonController.
[Serializable]
public class CharacterMover : MonoBehaviour {

	public float walkSpeed = 8f;
	public float runSpeed = 16f;

	public float jumpSpeed = 8f;
	public float gravityMultiplier = 5f;
	public float stickToGroundForce = 10f;

	public CharacterInput input;
	public CharacterController characterController;

	Vector3 currentMotion;
	CollisionFlags characterControllerCollisionFlag;

	void Start() {

		input = input ?? GetComponent<CharacterInput>();
		Debug.Assert(input != null);

		characterController = characterController ?? GetComponent<CharacterController>();
		Debug.Assert(characterController != null);	
	}

	void FixedUpdate() {
		
		Vector3 desiredMotion = GetDesiredMotion(input.moveAxes);
		UpdateCurrentMotion(desiredMotion);

		characterControllerCollisionFlag = characterController.Move(currentMotion * Time.fixedDeltaTime);
	}

	Vector3 GetDesiredMotion(Vector2 moveAxes) {
		
		var desiredMove = transform.forward * moveAxes.y + transform.right * moveAxes.x;

		// get a normal for the surface that is being touched to move along it
		RaycastHit hitInfo;
		Physics.SphereCast(transform.position, 
			characterController.radius, 
			Vector3.down,
			out hitInfo,
			characterController.height / 2f, 
			Physics.AllLayers, 
			QueryTriggerInteraction.Ignore
		);
		desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

		return desiredMove;
	}

	Vector3 UpdateCurrentMotion(Vector3 desiredMotion) {
		
		var speed = walkSpeed;

		currentMotion.x = desiredMotion.x * speed;
		currentMotion.z = desiredMotion.z * speed;
		currentMotion.y = GetVerticalMotionComponent();

		return currentMotion;
	}

	float GetVerticalMotionComponent() {
		
		if (characterController.isGrounded) {

			return input.isJump ? jumpSpeed : -stickToGroundForce;

		} else {

			return currentMotion.y + Physics.gravity.y * gravityMultiplier * Time.fixedDeltaTime;
		}
	}

	void OnControllerColliderHit(ControllerColliderHit hit) {
		
		Rigidbody body = hit.collider.attachedRigidbody;

		// dont move the rigidbody if the character is on top of it
		if (characterControllerCollisionFlag == CollisionFlags.Below) return;
		if (body == null || body.isKinematic) return;

		body.AddForceAtPosition(characterController.velocity * 0.1f, hit.point, ForceMode.Impulse);
	}
}
