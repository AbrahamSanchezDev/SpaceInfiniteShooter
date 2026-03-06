using UnityEngine;

public class SmoothCameraFollow : MonoBehaviour {
    public float smoothSpeed = 0.125f;
    public float leadFactor = 0.5f;

    private Transform myTransform;

    private void Awake() {
        myTransform = transform;
    }


    private void OnEnable() => EventHub.ShipMoved.AddListener(UpdateCamera);
    private void OnDisable() => EventHub.ShipMoved.RemoveListener(UpdateCamera);

    // Linked via UnityEvent in Inspector
    public void UpdateCamera(MovementData data) {
        Vector3 target = data.Position + ((Vector3)data.Velocity * leadFactor);
        target.z = -10;
        myTransform.position = Vector3.Lerp(myTransform.position, target, smoothSpeed);
    }
}