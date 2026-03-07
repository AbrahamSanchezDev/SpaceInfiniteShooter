using UnityEngine;

public class SiphonController : MonoBehaviour {
    [SerializeField] private InputReader input;
    [SerializeField] private float siphonPower = 25f;
    [SerializeField] private float range = 5f;
    [SerializeField] private LayerMask resourceLayer; // Set this to 'Resource' only

    private Transform myTransform;

    [Header("Visuals")]
    [SerializeField] private LineRenderer beamLine;

    private void Awake() {
        myTransform = transform;
        if (beamLine != null) beamLine.enabled = false;
    }

    private void Update() {
        if (input.IsSiphoning) {
            ExecuteSiphon();
        }
        else if (beamLine != null && beamLine.enabled) {
            beamLine.enabled = false;
        }
    }

    private void ExecuteSiphon() {
        RaycastHit2D hit = Physics2D.Raycast(myTransform.position, myTransform.up, range, resourceLayer);

        if (hit.collider != null && hit.collider.TryGetComponent(out ResourceController resource)) {
            resource.Siphon(siphonPower * Time.deltaTime);

            // Update Beam Visuals
            if (beamLine != null) {
                beamLine.enabled = true;
                beamLine.SetPosition(0, myTransform.position);
                beamLine.SetPosition(1, hit.point);
            }
        }
        else if (beamLine != null) {
            beamLine.enabled = false;
        }
    }
}