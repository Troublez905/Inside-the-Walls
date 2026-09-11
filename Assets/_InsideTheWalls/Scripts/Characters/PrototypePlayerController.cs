using UnityEngine;

namespace InsideTheWalls.Characters
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class PrototypePlayerController : MonoBehaviour
    {
        private const float MoveSpeed = 4.5f;
        private const float AnimatedWalkSpeed = 1.8f;
        private const float TurnSpeed = 12f;
        private const float Gravity = -24f;
        private const float GroundedVelocity = -2f;
        private const float CameraDistance = 6.5f;
        private const float CameraRadius = 0.28f;
        private const float CameraPadding = 0.12f;
        private const float GamepadLookSpeed = 105f;
        private const float MouseLookSensitivity = 3.2f;
        private static readonly int SpeedParameter = Animator.StringToHash("Speed");
        private readonly RaycastHit[] cameraHits = new RaycastHit[12];

        private CharacterController controller;
        private Transform cameraTransform;
        private float yaw;
        private float pitch = 18f;
        private float verticalVelocity;
        private bool movementSuppressed;
        private Animator characterAnimator;

        public bool InteractionPressed { get; private set; }
        public bool MovementSuppressed => movementSuppressed;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        internal Vector2? VerificationMoveInput { get; set; }
#endif

        public void SetMovementSuppressed(bool suppressed)
        {
            movementSuppressed = suppressed;
            if (suppressed && characterAnimator != null) characterAnimator.SetFloat(SpeedParameter, 0f);
        }

        public void TeleportTo(Vector3 worldPosition, float facingYawDegrees)
        {
            if (controller == null) controller = GetComponent<CharacterController>();
            bool wasEnabled = controller.enabled;
            controller.enabled = false;
            transform.SetPositionAndRotation(worldPosition, Quaternion.Euler(0f, facingYawDegrees, 0f));
            yaw = facingYawDegrees;
            verticalVelocity = GroundedVelocity;
            controller.enabled = wasEnabled;
            if (characterAnimator != null) characterAnimator.SetFloat(SpeedParameter, 0f);
        }

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            cameraTransform = Camera.main != null ? Camera.main.transform : null;
            characterAnimator = GetComponentInChildren<Animator>();
            if (characterAnimator != null && (!characterAnimator.enabled
                || characterAnimator.avatar == null || !characterAnimator.avatar.isValid
                || !characterAnimator.avatar.isHuman || characterAnimator.runtimeAnimatorController == null))
            {
                characterAnimator = null;
            }
            yaw = transform.eulerAngles.y;
            CharacterInteractionAnimator.Attach(transform);
        }

        private void Update()
        {
            if (cameraTransform == null && Camera.main != null) cameraTransform = Camera.main.transform;

            UpdateLook();
            UpdateMovement();
            InteractionPressed = Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.JoystickButton0);
            UpdateCamera();
        }

        private void UpdateLook()
        {
            Vector2 gamepadLook = new Vector2(Input.GetAxisRaw("Debug Horizontal"), Input.GetAxisRaw("Debug Vertical"));
            if (gamepadLook.sqrMagnitude < 0.04f) gamepadLook = Vector2.zero;
            else gamepadLook = Vector2.ClampMagnitude(gamepadLook, 1f);

            Vector2 mouseLook = Vector2.zero;
            if (Input.GetMouseButton(1))
            {
                mouseLook = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
            }

            yaw += gamepadLook.x * GamepadLookSpeed * Time.deltaTime + mouseLook.x * MouseLookSensitivity;
            pitch = Mathf.Clamp(
                pitch - gamepadLook.y * GamepadLookSpeed * 0.72f * Time.deltaTime - mouseLook.y * MouseLookSensitivity,
                8f,
                42f);
        }

        private void UpdateMovement()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Vector2 requestedMove = VerificationMoveInput
                ?? new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
#else
            Vector2 requestedMove = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
#endif
            Vector2 move = movementSuppressed
                ? Vector2.zero
                : Vector2.ClampMagnitude(requestedMove, 1f);
            Quaternion heading = Quaternion.Euler(0f, yaw, 0f);
            Vector3 direction = Vector3.ClampMagnitude(heading * new Vector3(move.x, 0f, move.y), 1f);

            if (controller.isGrounded && verticalVelocity < 0f) verticalVelocity = GroundedVelocity;
            else verticalVelocity += Gravity * Time.deltaTime;

            float moveSpeed = characterAnimator != null ? AnimatedWalkSpeed : MoveSpeed;
            Vector3 velocity = direction * moveSpeed;
            velocity.y = verticalVelocity;
            Vector3 positionBeforeMove = transform.position;
            CollisionFlags flags = controller.Move(velocity * Time.deltaTime);
            if ((flags & CollisionFlags.Below) != 0 && verticalVelocity < 0f) verticalVelocity = GroundedVelocity;

            if (characterAnimator != null)
            {
                Vector3 displacement = transform.position - positionBeforeMove;
                displacement.y = 0f;
                float normalizedSpeed = !movementSuppressed && Time.deltaTime > 0f
                    ? Mathf.Clamp01(displacement.magnitude / (moveSpeed * Time.deltaTime))
                    : 0f;
                // Collision response, not held input, determines whether Noah is walking.
                if (normalizedSpeed <= 0.01f) characterAnimator.SetFloat(SpeedParameter, 0f);
                else characterAnimator.SetFloat(SpeedParameter, normalizedSpeed, 0.12f, Time.deltaTime);
            }

            if (direction.sqrMagnitude > 0.01f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * TurnSpeed);
            }
        }

        private void UpdateCamera()
        {
            if (cameraTransform == null) return;

            Quaternion orbit = Quaternion.Euler(pitch, yaw, 0f);
            Vector3 focus = transform.position + Vector3.up * 1.4f;
            Vector3 backward = -(orbit * Vector3.forward);
            float resolvedDistance = ResolveCameraDistance(focus, backward);
            cameraTransform.position = focus + backward * resolvedDistance;
            cameraTransform.rotation = Quaternion.LookRotation(focus - cameraTransform.position, Vector3.up);
        }

        private float ResolveCameraDistance(Vector3 focus, Vector3 direction)
        {
            int hitCount = Physics.SphereCastNonAlloc(
                focus,
                CameraRadius,
                direction,
                cameraHits,
                CameraDistance,
                Physics.DefaultRaycastLayers,
                QueryTriggerInteraction.Ignore);
            if (hitCount == cameraHits.Length)
            {
                return ResolveCameraDistanceFromAllHits(focus, direction);
            }

            return NearestCameraHitDistance(cameraHits, hitCount);
        }

        private float ResolveCameraDistanceFromAllHits(Vector3 focus, Vector3 direction)
        {
            RaycastHit[] allHits = Physics.SphereCastAll(
                focus,
                CameraRadius,
                direction,
                CameraDistance,
                Physics.DefaultRaycastLayers,
                QueryTriggerInteraction.Ignore);
            return NearestCameraHitDistance(allHits, allHits.Length);
        }

        private float NearestCameraHitDistance(RaycastHit[] hits, int hitCount)
        {
            float nearest = CameraDistance;
            for (int i = 0; i < hitCount; i++)
            {
                Collider hitCollider = hits[i].collider;
                if (ShouldIgnoreCameraHit(hitCollider)) continue;
                nearest = Mathf.Min(nearest, hits[i].distance);
            }

            return Mathf.Clamp(nearest - CameraPadding, 0.55f, CameraDistance);
        }

        private bool ShouldIgnoreCameraHit(Collider hitCollider)
        {
            if (hitCollider == null || hitCollider.transform == transform || hitCollider.transform.IsChildOf(transform)) return true;
            Vector3 focus = transform.position + Vector3.up * 1.4f;
            if (hitCollider.bounds.max.y < focus.y - 0.25f) return true;
            return hitCollider.name.StartsWith("Objective ", System.StringComparison.Ordinal);
        }
    }
}
