using UnityEngine;

namespace InsideTheWalls.Presentation
{
    internal sealed class ObjectiveMarkerBillboard : MonoBehaviour
    {
        private Vector3 basePosition;
        private float phase;

        private void Awake()
        {
            basePosition = transform.position;
            phase = transform.position.sqrMagnitude * 0.17f;
        }

        private void LateUpdate()
        {
            Camera targetCamera = Camera.main;
            if (targetCamera != null) transform.rotation = targetCamera.transform.rotation;
            transform.position = basePosition + Vector3.up * (Mathf.Sin(Time.unscaledTime * 2.2f + phase) * 0.08f);
        }
    }
}
