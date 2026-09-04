using UnityEngine;

namespace InsideTheWalls.Characters
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class PrototypePlayerController : MonoBehaviour
    {
        private CharacterController controller;
        private Transform cameraTransform;
        private float yaw;
        private float pitch = 18f;

        public bool InteractionPressed { get; private set; }

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            cameraTransform = Camera.main != null ? Camera.main.transform : null;
        }

        private void Update()
        {
            Vector2 move = ReadMove();
            Vector2 look = ReadLook();
            yaw += look.x * 90f * Time.deltaTime;
            pitch = Mathf.Clamp(pitch - look.y * 70f * Time.deltaTime, 8f, 42f);

            Vector3 forward = Quaternion.Euler(0f, yaw, 0f) * Vector3.forward;
            Vector3 right = Quaternion.Euler(0f, yaw, 0f) * Vector3.right;
            Vector3 direction = Vector3.ClampMagnitude(forward * move.y + right * move.x, 1f);
            controller.SimpleMove(direction * 4.5f);

            if (direction.sqrMagnitude > 0.01f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 12f);
            }

            InteractionPressed = Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.JoystickButton0);

            UpdateCamera();
        }

        private Vector2 ReadMove()
        {
            return Vector2.ClampMagnitude(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")), 1f);
        }

        private static Vector2 ReadLook()
        {
            Vector2 value = Vector2.zero;
            if (Input.GetMouseButton(1))
            {
                value += new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * 0.35f;
            }

            return value;
        }

        private void UpdateCamera()
        {
            if (cameraTransform == null)
            {
                return;
            }

            Quaternion orbit = Quaternion.Euler(pitch, yaw, 0f);
            Vector3 focus = transform.position + Vector3.up * 1.4f;
            cameraTransform.position = focus - orbit * Vector3.forward * 6.5f;
            cameraTransform.rotation = Quaternion.LookRotation(focus - cameraTransform.position, Vector3.up);
        }
    }
}
