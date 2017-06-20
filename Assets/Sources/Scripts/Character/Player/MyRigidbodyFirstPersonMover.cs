using System;
using UnityEngine;

/// A rigidbody-based first person mover I started to adapt 
/// from the standard assets but eventually abandoned.
public class MyRigidbodyFirstPersonMover : MonoBehaviour {
	
	[Serializable]
	public class MovementSettings
	{
		public float ForwardSpeed = 8.0f;   // Speed when walking forward
		public float BackwardSpeed = 4.0f;  // Speed when walking backwards
		public float StrafeSpeed = 4.0f;    // Speed when walking sideways
		public float RunMultiplier = 2.0f;   // Speed when sprinting
		public KeyCode RunKey = KeyCode.LeftShift;
		public float JumpForce = 30f;
		public AnimationCurve SlopeCurveModifier = new AnimationCurve(new Keyframe(-90.0f, 1.0f), new Keyframe(0.0f, 1.0f), new Keyframe(90.0f, 0.0f));
		[HideInInspector] public float CurrentTargetSpeed = 8f;

		#if !MOBILE_INPUT
		private bool m_Running;
		#endif

		public void UpdateDesiredTargetSpeed(Vector2 input)
		{
			if (input == Vector2.zero) return;
			if (input.x > 0 || input.x < 0)
			{
				//strafe
				CurrentTargetSpeed = StrafeSpeed;
			}
			if (input.y < 0)
			{
				//backwards
				CurrentTargetSpeed = BackwardSpeed;
			}
			if (input.y > 0)
			{
				//forwards
				//handled last as if strafing and moving forward at the same time forwards speed should take precedence
				CurrentTargetSpeed = ForwardSpeed;
			}
			#if !MOBILE_INPUT
			if (Input.GetKey(RunKey))
			{
				CurrentTargetSpeed *= RunMultiplier;
				m_Running = true;
			}
			else
			{
				m_Running = false;
			}
			#endif
		}

		#if !MOBILE_INPUT
		public bool Running
		{
			get { return m_Running; }
		}
		#endif
	}

	[Serializable]
	public class AdvancedSettings
	{
		public float groundCheckDistance = 0.01f; // distance for checking if the controller is grounded ( 0.01f seems to work best for this )
		public float stickToGroundHelperDistance = 0.5f; // stops the character
		public float slowDownRate = 20f; // rate at which the controller comes to a stop when there is no input
		public bool airControl; // can the user control the direction that is being moved in the air
		[Tooltip("set it to 0.1 or more if you get stuck in wall")]
		public float shellOffset; //reduce the radius by that ratio to avoid getting stuck in wall (a value of 0.1f is nice)
	}

	public MovementSettings movementSettings = new MovementSettings();
	public AdvancedSettings advancedSettings = new AdvancedSettings();

	[SerializeField] CharacterInput input;
	[SerializeField] Transform cameraTransform;

	Rigidbody rigidBody;
	CapsuleCollider capsuleCollider;
	Vector3 groundNormal;
	bool willJump, wasGrounded, isJumping, isGrounded;

	public Vector3 Velocity {
		
		get { return rigidBody.velocity; }
	}

	public bool Grounded {
		get { return isGrounded; }
	}

	public bool Jumping {
		get { return isJumping; }
	}

	public bool Running {
		get {
			#if !MOBILE_INPUT
			return movementSettings.Running;
			#else
			return false;
			#endif
		}
	}

	void Start() {
		
		input = input ?? GetComponentInParent<CharacterInput>();
		Debug.Assert(input != null);

		cameraTransform = cameraTransform ?? GetComponentInChildren<Camera>().transform;
		Debug.Assert(cameraTransform != null);

		rigidBody = GetComponent<Rigidbody>();
		capsuleCollider = GetComponent<CapsuleCollider>();
	}

	void Update() {
		
		if (input.isJump) {
			willJump = true;
		}
	}
		
	private void FixedUpdate() {
		
		CheckGround();
		var moveAxes = GetMoveAxes();

		var isMoveZero = (Mathf.Abs(moveAxes.x) <= float.Epsilon && Mathf.Abs(moveAxes.y) <= float.Epsilon);
		if (!isMoveZero && (isGrounded || advancedSettings.airControl)) {
			
			// always move along the camera forward as it is the direction that it being aimed at
			Vector3 desiredMove = cameraTransform.forward * moveAxes.y + cameraTransform.right * moveAxes.x;
			desiredMove = Vector3.ProjectOnPlane(desiredMove, groundNormal).normalized;
			desiredMove *= movementSettings.CurrentTargetSpeed;
			 
			if (rigidBody.velocity.sqrMagnitude <
				(movementSettings.CurrentTargetSpeed * movementSettings.CurrentTargetSpeed)) {

				rigidBody.AddForce(desiredMove * GetSlopeMultiplier(), ForceMode.Impulse);
			}
		}

		if (isGrounded) {
			
			rigidBody.drag = 5f;

			if (willJump) {
				
				rigidBody.drag = 0f;
				rigidBody.velocity = new Vector3(rigidBody.velocity.x, 0f, rigidBody.velocity.z);
				rigidBody.AddForce(new Vector3(0f, movementSettings.JumpForce, 0f), ForceMode.Impulse);
				isJumping = true;
			}

			if (!isJumping && isMoveZero && rigidBody.velocity.magnitude < 1f) {
				rigidBody.Sleep();
			}

		} else {
			
			rigidBody.drag = 0f;
			if (wasGrounded && !isJumping) {
				StickToGroundHelper();
			}
		}

		willJump = false;
	}
		
	float GetSlopeMultiplier() {
		
		float angle = Vector3.Angle(groundNormal, Vector3.up);
		return movementSettings.SlopeCurveModifier.Evaluate(angle);
	}

	void StickToGroundHelper() {
		
		RaycastHit hitInfo;
		if (Physics.SphereCast(transform.position, capsuleCollider.radius * (1.0f - advancedSettings.shellOffset), Vector3.down, out hitInfo,
			((capsuleCollider.height/2f) - capsuleCollider.radius) +
			advancedSettings.stickToGroundHelperDistance, Physics.AllLayers, QueryTriggerInteraction.Ignore))
		{
			if (Mathf.Abs(Vector3.Angle(hitInfo.normal, Vector3.up)) < 85f)
			{
				rigidBody.velocity = Vector3.ProjectOnPlane(rigidBody.velocity, hitInfo.normal);
			}
		}
	}

	Vector2 GetMoveAxes() {

		var moveAxes = input.moveAxes;
		movementSettings.UpdateDesiredTargetSpeed(moveAxes);
		return moveAxes;
	}

	/// Sphere cast down just beyond the bottom of the capsule to see if the capsule is colliding round the bottom
	void CheckGround() {
		
		wasGrounded = isGrounded;
		RaycastHit hitInfo;
		bool didHit = Physics.SphereCast(
			transform.position, 
			capsuleCollider.radius * (1.0f - advancedSettings.shellOffset), 
			Vector3.down, 
			out hitInfo,
			((capsuleCollider.height/2f) - capsuleCollider.radius) + advancedSettings.groundCheckDistance, 
			Physics.AllLayers, 
			QueryTriggerInteraction.Ignore
		);

		if (didHit) {
			
			isGrounded = true;
			groundNormal = hitInfo.normal;
		} else {
			
			isGrounded = false;
			groundNormal = Vector3.up;
		}

		if (!wasGrounded && isGrounded && isJumping) {
			isJumping = false;
		}
	}
}
