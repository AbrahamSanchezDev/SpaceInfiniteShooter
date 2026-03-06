using UnityEngine;

public class SiphonController : MonoBehaviour {
    [SerializeField] private InputReader input;
    [SerializeField] private float siphonPower = 25f;
    [SerializeField] private float range = 5f;
    [SerializeField] private LayerMask resourceLayer; // Set this to 'Resource' only

    private Transform myTransform;

    private void Awake() {
        myTransform = transform;
    }

    private void Update() {
        if (input.IsSiphoning) {
            ExecuteSiphon();
        }
    }

    private void ExecuteSiphon() {
        // 1. We provide the LayerMask here
        // 2. We also ensure the ray starts slightly 'ahead' of the ship or ignore self
        RaycastHit2D hit = Physics2D.Raycast(myTransform.position, myTransform.up, range, resourceLayer);

        if (hit.collider != null) {
            // Using TryGetComponent is the safest/most performant way to check for the controller
            if (hit.collider.TryGetComponent(out ResourceController resource)) {
                resource.Siphon(siphonPower * Time.deltaTime);

                // Optional: Draw a line in the editor to see it working
                Debug.DrawLine(myTransform.position, hit.point, Color.cyan);
            }
        }
    }
}