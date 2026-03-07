using UnityEngine;
using UnityEngine.UI;

public class RadarIndicator : MonoBehaviour {
    [SerializeField] private Transform arrowIcon;
    [SerializeField] private float hideDistance = 5f;
    [SerializeField] private LayerMask targetLayer;

    private Transform _playerTransform;
    private Collider2D[] _results = new Collider2D[10];

    [SerializeField] private GameObject resourcesArrow;
    [SerializeField] private GameObject enemiesArrow;
    private ContactFilter2D maskFilter;
    private float detectRadius = 50f;

    private void Awake() {
        maskFilter = new ContactFilter2D {
            layerMask = targetLayer,
            useLayerMask = true
        };
    }
    private void Start() {
        // Simple way to find player in Drift mode
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update() {
        if (_playerTransform == null) return;

        // Find targets in a large radius  

        int count = Physics2D.OverlapCircle(_playerTransform.position, detectRadius, maskFilter, _results);


        if (count > 0) {
            arrowIcon.gameObject.SetActive(true);
            Transform nearest = GetNearest(count);
            RotateArrow(nearest.position);
        }
        else {
            arrowIcon.gameObject.SetActive(false);
        }
    }

    private Transform GetNearest(int count) {
        Transform bestTarget = null;
        float closestDist = Mathf.Infinity;

        for (int i = 0; i < count; i++) {
            float dist = Vector3.Distance(_playerTransform.position, _results[i].transform.position);
            if (dist < closestDist && dist > hideDistance) {
                closestDist = dist;
                bestTarget = _results[i].transform;
            }
        }
        if (bestTarget != null) {
            ShowValidArrow(bestTarget);
        }

        return bestTarget;
    }
    private void ShowValidArrow(Transform bestTarget) {
        if (enemiesArrow)
            enemiesArrow.SetActive(bestTarget.tag == "Enemy");
        if (resourcesArrow)
            resourcesArrow.SetActive(bestTarget.tag == "Resources");
    }

    private void RotateArrow(Vector3 targetPos) {
        Vector2 dir = (targetPos - _playerTransform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        arrowIcon.rotation = Quaternion.Euler(0, 0, angle - 90f);
    }
}