using UnityEngine;

public class VFXManager : MonoBehaviour {
    public static VFXManager Instance { get; private set; }

    [SerializeField] private ObjectPooler explosionPool;
    [SerializeField] private ObjectPooler hitSparkPool;

    private void Awake() {
        if (Instance == null) Instance = this;
    }

    public void SpawnExplosion(Vector3 position) => SpawnFromPool(explosionPool, position);
    public void SpawnHitSpark(Vector3 position) => SpawnFromPool(hitSparkPool, position);

    private void SpawnFromPool(ObjectPooler pool, Vector3 pos) {
        GameObject vfx = pool.GetObject();
        if (vfx != null) {
            vfx.transform.position = pos;
            vfx.SetActive(true);
        }
    }
}