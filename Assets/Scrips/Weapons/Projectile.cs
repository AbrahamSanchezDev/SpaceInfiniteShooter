using UnityEngine;

public class Projectile : MonoBehaviour {
    [SerializeField] private float damage = 10f;
    [SerializeField] private float lifetime = 2f;
    private int _targetLayer;

    public void SetTargetLayer(int layer) => _targetLayer = layer;

    private void OnEnable() => Invoke(nameof(Deactivate), lifetime);
    private void OnDisable() => CancelInvoke();

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.layer == _targetLayer) {

            if (collision.TryGetComponent(out EnemyController enemy)) {
                // Apply damage logic here
                enemy.gameObject.SetActive(false);
            }
            // Do damage logic

            Deactivate();
        }
    }

    private void Deactivate() => gameObject.SetActive(false);
}