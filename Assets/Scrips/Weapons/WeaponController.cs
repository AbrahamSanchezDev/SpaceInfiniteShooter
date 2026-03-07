using UnityEngine;

public class WeaponController : MonoBehaviour {
    [Header("Weapon Settings")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private float bulletSpeed = 20f;
    [SerializeField] private string targetLayerName; // "Enemy" for player, "Player" for enemy

    [SerializeField] private int projectileDamage = 20;

    private float _nextFireTime;
    private Transform _myTransform;

    private void Awake() => _myTransform = transform;

    public void RequestFire() {
        if (Time.time >= _nextFireTime) {
            ExecuteShot();
            _nextFireTime = Time.time + fireRate;
        }
    }

    private void ExecuteShot() {
        GameObject bullet = ProjectilePool.Instance.GetBullet();
        if (bullet != null) {
            bullet.transform.position = firePoint.position;
            bullet.transform.rotation = _myTransform.rotation;
            bullet.SetActive(true);

            if (bullet.TryGetComponent(out Rigidbody2D rb)) {
                rb.linearVelocity = _myTransform.up * bulletSpeed;
            }

            // Set the bullet's target layer so it doesn't hit the shooter
            if (bullet.TryGetComponent(out Projectile p)) {
                p.SetTargetLayer(LayerMask.NameToLayer(targetLayerName));
                p.SetDamage(projectileDamage);
            }
        }
    }
}