using UnityEngine;

public class ProjectilePool : MonoBehaviour {
    public static ProjectilePool Instance { get; private set; }
    [SerializeField] private ObjectPooler pool;

    private void Awake() {
        if (Instance == null) Instance = this;
    }

    public GameObject GetBullet() => pool.GetObject();
}