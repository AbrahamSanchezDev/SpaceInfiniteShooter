using UnityEngine;

public class SmoothCameraFollow : MonoBehaviour {
    [Header("Follow Settings")]
    [SerializeField] private float smoothSpeed = 0.125f;
    [SerializeField] private float leadFactor = 0.5f;
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);

    private Transform myTransform;
    private Vector3 targetPosition;

    private void Awake() {
        myTransform = transform;
    }

    private void OnEnable() {
        // Subscribe to movement and origin reset events
        EventHub.ShipMoved.AddListener(OnPlayerMoved);
        EventHub.OriginResetRequested.AddListener(OnOriginReset);
    }

    private void OnDisable() {
        // Unsubscribe to prevent memory leaks
        EventHub.ShipMoved.RemoveListener(OnPlayerMoved);
        EventHub.OriginResetRequested.RemoveListener(OnOriginReset);
    }

    private void OnPlayerMoved(MovementData data) {
        // Calculate the look-ahead (lead) based on ship velocity
        Vector3 lead = (Vector3)data.Velocity * leadFactor;

        // Update the target we want to reach
        targetPosition = data.Position + lead + offset;
    }

    private void OnOriginReset(Vector3 offsetShift) {
        // Instantly teleport the camera and the target position 
        // to maintain frame-perfect synchronization
        myTransform.position -= offsetShift;
        targetPosition -= offsetShift;
    }

    private void LateUpdate() {
        // Smoothly move the camera toward the targetPosition
        // We use LateUpdate to ensure the ship has finished moving for the frame
        Vector3 smoothedPosition = Vector3.Lerp(myTransform.position, targetPosition, smoothSpeed);
        myTransform.position = smoothedPosition;
    }
}