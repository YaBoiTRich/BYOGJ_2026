using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	[RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM
	[RequireComponent(typeof(PlayerInput))]
#endif
	public class FirstPersonControllerZeroG : MonoBehaviour
	{
		[Header("Player")]
		[Tooltip("Move speed of the character in m/s")]
		public float MoveSpeed = 4.0f;
		[Tooltip("Sprint speed of the character in m/s")]
		public float SprintSpeed = 6.0f;
		[Tooltip("Rotation speed of the character")]
		public float RotationSpeed = 1.0f;
		[Tooltip("Acceleration and deceleration")]
		public float SpeedChangeRate = 10.0f;

		public Rigidbody m_Rigidbody;
		public float m_Thrust = 20f;
		public float m_PullForce = 25f;

		[Space(10)]
		[Tooltip("The height the player can jump")]
		public float JumpHeight = 1.2f;
		[Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
		public float Gravity = -15.0f;

		[Space(10)]
		[Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
		public float JumpTimeout = 0.1f;
		[Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
		public float FallTimeout = 0.15f;

		[Header("Player Grounded")]
		[Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
		public bool Grounded = true;
		[Tooltip("Useful for rough ground")]
		public float GroundedOffset = -0.14f;
		[Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
		public float GroundedRadius = 0.5f;
		[Tooltip("What layers the character uses as ground")]
		public LayerMask GroundLayers;

		[Header("Cinemachine")]
		[Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
		public GameObject CinemachineCameraTarget;
		[Tooltip("How far in degrees can you move the camera up")]
		public float TopClamp = 90.0f;
		[Tooltip("How far in degrees can you move the camera down")]
		public float BottomClamp = -90.0f;

		[Header("Raycasting Settings")] 
		public float raycastDistance;
		public LineRenderer lineRenderer;
		public GameObject lineRendererStart;
		
		// cinemachine
		private float _cinemachineTargetPitch;

		// player
		private float _speed;
		private float _rotationVelocity;
		private float _verticalVelocity;
		private float _terminalVelocity = 53.0f;

		// timeout deltatime
		private float _jumpTimeoutDelta;
		private float _fallTimeoutDelta;

	
#if ENABLE_INPUT_SYSTEM
		private PlayerInput _playerInput;
#endif
		private CharacterController _controller;
		private StarterAssetsInputs _input;
		private GameObject _mainCamera;

		private const float _threshold = 0.01f;

		private bool IsCurrentDeviceMouse
		{
			get
			{
				#if ENABLE_INPUT_SYSTEM
				return _playerInput.currentControlScheme == "KeyboardMouse";
				#else
				return false;
				#endif
			}
		}
		
		LayerMask layerMask;
		LayerMask finalMask;

		private void Awake()
		{
			layerMask = LayerMask.GetMask("Ignore Raycast");
			finalMask = ~layerMask; 	
			
			// get a reference to our main camera
			if (_mainCamera == null)
			{
				_mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
			}
		}

		private void Start()
		{
			_controller = GetComponent<CharacterController>();
			_input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM
			_playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

			
			// reset our timeouts on start
			_jumpTimeoutDelta = JumpTimeout;
			_fallTimeoutDelta = FallTimeout;
		}

		private void Update()
		{
		}

		private void FixedUpdate()
		{
			RaycastHandler();
			
			// normalise input direction
			Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

			// note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
			// if there is a move input rotate player when the player is moving
			if (_input.move != Vector2.zero)
			{
				// move
				inputDirection = CinemachineCameraTarget.transform.right * _input.move.x + CinemachineCameraTarget.transform.forward * _input.move.y;
			}
			
			m_Rigidbody.AddForce(inputDirection * m_Thrust);
			
		}

		private void RaycastHandler()
		{
			RaycastHit hit;
			
			if (Physics.Raycast(_mainCamera.transform.position, _mainCamera.transform.TransformDirection(Vector3.forward), out hit, raycastDistance, finalMask))
			{
				lineRenderer.enabled = true;
				lineRenderer.SetPosition(0, lineRendererStart.transform.position);
				lineRenderer.SetPosition(1, hit.point);
				// Debug.DrawRay(_mainCamera.transform.position, _mainCamera.transform.TransformDirection(Vector3.forward) * raycastDistance, Color.yellow);
				
				if (_playerInput.actions["Primary"].IsPressed())
				{
					var heading = (hit.point - lineRendererStart.transform.position).normalized;
					m_Rigidbody.AddForce(_mainCamera.transform.forward + heading * m_PullForce);
				}

				if (_playerInput.actions["Primary"].WasReleasedThisFrame())
				{
					m_Rigidbody.linearVelocity = Vector3.zero;
					
				}
				
				// var time = Time.deltaTime;
				// Debug.Log(time + ": Did Hit");
			}
			else
			{
				lineRenderer.enabled = false;
			}
		}

		private void LateUpdate()
		{
			CameraRotation();
		}
		
		private void CameraRotation()
		{
			// if there is an input
			if (_input.look.sqrMagnitude >= _threshold)
			{
				//Don't multiply mouse input by Time.deltaTime
				float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;
				
				_cinemachineTargetPitch += _input.look.y * RotationSpeed * deltaTimeMultiplier;
				_rotationVelocity = _input.look.x * RotationSpeed * deltaTimeMultiplier;

				// clamp our pitch rotation
				_cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

				// Update Cinemachine camera target pitch
				CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);

				// rotate the player left and right
				transform.Rotate(Vector3.up * _rotationVelocity);
			}
		}

		private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
		{
			if (lfAngle < -360f) lfAngle += 360f;
			if (lfAngle > 360f) lfAngle -= 360f;
			return Mathf.Clamp(lfAngle, lfMin, lfMax);
		}

		private void OnDrawGizmosSelected()
		{
			Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
			Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

			if (Grounded) Gizmos.color = transparentGreen;
			else Gizmos.color = transparentRed;

			// when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
			Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
		}
	}
}