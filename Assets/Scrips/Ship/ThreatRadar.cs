using UnityEngine;
using UnityEngine.Events;

public class ThreatRadar : MonoBehaviour {
    [Header("Radar Settings")]
    [SerializeField] private float detectionRadius = 15f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float detectionThreshold = 1.0f; // Time required to trigger Strike Mode

    private float _detectionTimer = 0f;
    private bool _isInCombat = false;
    private Transform _myTransform;

    // Buffer for overlap results to avoid allocation
    private Collider2D[] _results = new Collider2D[5];

    private void Awake() => _myTransform = transform;

    private void Update() {
        CheckForThreats();
    }

    private void CheckForThreats() {
        // Non-allocating overlap check
        int count = Physics2D.OverlapCircleNonAlloc(_myTransform.position, detectionRadius, _results, enemyLayer);

        if (count > 0 && !_isInCombat) {
            _detectionTimer += Time.deltaTime;

            // Optional: Broadcast "Warning" event for UI/VFX here
            if (_detectionTimer >= detectionThreshold) {
                EnterStrikeMode();
            }
        }
        else if (count == 0 && _isInCombat) {
            // Optional: Logic to exit combat if enemies are cleared or outrun
            _detectionTimer = 0f;
        }
    }

    private void EnterStrikeMode() {
        _isInCombat = true;
        _detectionTimer = 0f;
        EventHub.StrikeModeStarted.Invoke();
        Debug.Log("<color=red>STRIKE MODE ACTIVATED</color>");
    }

    // Call this via EventHub when all enemies are destroyed
    public void ExitStrikeMode() {
        _isInCombat = false;
        EventHub.StrikeModeEnded.Invoke();
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}