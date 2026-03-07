using UnityEngine;

public class BackgroundDecorator : MonoBehaviour {
    [Header("Sprite Data")]
    [SerializeField] private Sprite[] planetSprites;
    [SerializeField] private Sprite[] shootingStarSprites;

    [Header("Settings")]
    [SerializeField] private int maxPlanetsPerChunk = 2;
    [SerializeField] private int maxShootingStars = 1;
    [SerializeField] private float chunkHalfSize = 24f; // Stay slightly inside the 50x50 area

    // We use a simple local list to track objects so we can disable them on disable
    private GameObject[] _spawnedObjects;
    private const int TOTAL_POOL_SIZE = 10; // Adjust based on max settings

    private void Awake() {
        InitializePool();
    }

    private void InitializePool() {
        _spawnedObjects = new GameObject[TOTAL_POOL_SIZE];
        for (int i = 0; i < TOTAL_POOL_SIZE; i++) {
            GameObject go = new GameObject("BG_Object_" + i);
            go.transform.SetParent(transform);
            go.AddComponent<SpriteRenderer>();
            go.SetActive(false);
            _spawnedObjects[i] = go;
        }
    }

    private void OnEnable() {
        RandomizeBackground();
    }

    private void RandomizeBackground() {
        // Deactivate all first
        foreach (var obj in _spawnedObjects) obj.SetActive(false);

        int currentIndex = 0;

        // 1. Spawn Random Planets
        int planetCount = Random.Range(0, maxPlanetsPerChunk + 1);
        for (int i = 0; i < planetCount; i++) {
            if (currentIndex >= _spawnedObjects.Length) break;
            SetupObject(_spawnedObjects[currentIndex], planetSprites, 0.1f, 1.2f, -1); // Layer -1 for BG
            currentIndex++;
        }

        // 2. Spawn Shooting Stars
        int starCount = Random.Range(0, maxShootingStars + 1);
        for (int i = 0; i < starCount; i++) {
            if (currentIndex >= _spawnedObjects.Length) break;
            SetupObject(_spawnedObjects[currentIndex], shootingStarSprites, 1f, 2f, -2); // Further back
            currentIndex++;
        }
    }

    private void SetupObject(GameObject go, Sprite[] pool, float minScale, float maxScale, int sortingOrder) {
        if (pool == null || pool.Length == 0) return;

        SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
        sr.sprite = pool[Random.Range(0, pool.Length)];
        sr.sortingOrder = sortingOrder;

        // Random Position within the 50x50 chunk
        go.transform.localPosition = new Vector3(
            Random.Range(-chunkHalfSize, chunkHalfSize),
            Random.Range(-chunkHalfSize, chunkHalfSize),
            0
        );

        // Random Rotation and Scale
        go.transform.localRotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));
        float scale = Random.Range(minScale, maxScale);
        go.transform.localScale = new Vector3(scale, scale, 1);

        go.SetActive(true);
    }
}