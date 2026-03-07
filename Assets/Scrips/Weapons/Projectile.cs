using UnityEngine;

public class Projectile : MonoBehaviour {
    [SerializeField] private int damage = 10;
    [SerializeField] private float lifetime = 2f;
    private int _targetLayer;

    public void SetTargetLayer(int layer) => _targetLayer = layer;

    private void OnEnable() => Invoke(nameof(Deactivate), lifetime);
    private void OnDisable() => CancelInvoke();

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.layer == _targetLayer) {

            if (collision.TryGetComponent(out EnemyController enemy)) {
                enemy.TakeDamage(damage, collision.transform.position);
            }
            // Do damage logic
            else if (collision.TryGetComponent(out ShipController player)) {
                GlobalGameData.Instance.Scrap -= damage;
            }
            Deactivate();
        }
    }

    private void Deactivate() => gameObject.SetActive(false);

    public void SetDamage(int dmg) {
        damage = dmg;
    }
}