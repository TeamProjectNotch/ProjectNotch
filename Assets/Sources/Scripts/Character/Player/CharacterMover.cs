using System;
using UnityEngine;

/// Moves the character contoller based on inputs in the provided CharacterInput.
/// **Very** roughly based on UnityStandardAssets.Characters.FirstPerson.FirstPersonController.
[Serializable]
public class CharacterMover : MonoBehaviour, ICharacterBehaviour {

	public float walkSpeed = 8f;
	public float runSpeed = 16f;

	public float jumpSpeed = 8f;
	public float gravityMultiplier = 5f;
	public float stickToGroundForce = 10f;

	public CharacterController characterController;
	public Vector3 velocity {get; set;}

	PlayerInputState inputState;
	Vector3 groundNormal;

	CollisionFlags characterControllerCollisionFlag;

	void Start() {

		characterController = characterController ?? GetComponent<CharacterController>();
		Debug.Assert(characterController != null);	
	}

	void FixedUpdate() {

		groundNormal = GetGroundNormal();
	}

	public void SimulateStep(PlayerInputState inputState) {

		this.inputState = inputState;

		var dt = Time.fixedDeltaTime;
		velocity = GetNewVelocity(inputState);

		var deltaPosition = velocity * dt + 0.5f * Physics.gravity * dt * dt;
		characterControllerCollisionFlag = characterController.Move(deltaPosition);

		velocity += Physics.gravity * dt;

		Debug.Log(velocity);
	}

	Vector3 GetNewVelocity(PlayerInputState inputState) {
		
		var desiredVelocity = GetDesiredMotion(inputState.moveAxes) * walkSpeed;
		desiredVelocity.y = velocity.y;

		if (characterController.isGrounded) {
			
			desiredVelocity.y = inputState.buttonPressedJump ? jumpSpeed : -stickToGroundForce;
		}

		return desiredVelocity;
	}

	Vector3 GetDesiredMotion(Vector2 moveAxes) {
		
		var desiredMove = transform.forward * moveAxes.y + transform.right * moveAxes.x;

		desiredMove = Vector3.ProjectOnPlane(desiredMove, groundNormal).normalized;

		return desiredMove;
	}

	Vector3 GetGroundNormal() {

		RaycastHit hitInfo;
		bool didCollide = Physics.SphereCast(transform.position, 
			characterController.radius, 
			Vector3.down,
			out hitInfo,
			characterController.height / 2f, 
			Physics.AllLayers, 
			QueryTriggerInteraction.Ignore
		);

		return didCollide ? hitInfo.normal : Vector3.up;
	}

	void OnControllerColliderHit(ControllerColliderHit hit) {
		
		Rigidbody body = hit.collider.attachedRigidbody;

		// dont move the rigidbody if the character is on top of it
		if (characterControllerCollisionFlag == CollisionFlags.Below) return;
		if (body == null || body.isKinematic) return;

		body.AddForceAtPosition(characterController.velocity * 0.1f, hit.point, ForceMode.Impulse);
	}
}
