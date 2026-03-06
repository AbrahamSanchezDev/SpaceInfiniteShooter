using System.Collections;
using UnityEngine;

public class SmoothCameraFollow : MonoBehaviour {
    [Header("Follow Settings")]
    [SerializeField] private float smoothSpeed = 0.125f;
    [SerializeField] private float leadFactor = 0.5f;
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);

    private Transform myTransform;
    private Vector3 targetPosition;

    [Header("Combat Zoom")]
    [SerializeField] private float driftZoom = 10f;
    [SerializeField] private float strikeZoom = 7f; // Closer for combat
    private Camera _cam;

    private void Awake() {
        myTransform = transform;
        _cam = GetComponent<Camera>();
        _cam.orthographicSize = driftZoom;
    }

    private void OnEnable() {
        // Subscribe to movement and origin reset events
        EventHub.ShipMoved.AddListener(OnPlayerMoved);
        EventHub.OriginResetRequested.AddListener(OnOriginReset);
        EventHub.StrikeModeStarted.AddListener(ZoomIn);
        EventHub.StrikeModeEnded.AddListener(ZoomOut);
    }

    private void OnDisable() {
        // Unsubscribe to prevent memory leaks
        EventHub.ShipMoved.RemoveListener(OnPlayerMoved);
        EventHub.OriginResetRequested.RemoveListener(OnOriginReset);
        EventHub.StrikeModeStarted.RemoveListener(ZoomIn);
        EventHub.StrikeModeEnded.RemoveListener(ZoomOut);
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


    private void ZoomIn() {
        StartCoroutine(TransitionZoom(strikeZoom));
    }
    private void ZoomOut() {
        StartCoroutine(TransitionZoom(driftZoom));
    }

    private IEnumerator TransitionZoom(float targetSize) {
        float duration = 0.5f;
        float elapsed = 0f;
        float startSize = _cam.orthographicSize;

        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            _cam.orthographicSize = Mathf.Lerp(startSize, targetSize, elapsed / duration);
            yield return null;
        }
        _cam.orthographicSize = targetSize;
    }
}