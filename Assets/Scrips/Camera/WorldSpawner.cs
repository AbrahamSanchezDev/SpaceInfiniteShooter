using System.Collections.Generic;
using UnityEngine;

public class WorldSpawner : MonoBehaviour {
    public ObjectPooler chunkPool;
    public float chunkSize = 50f;
    public int viewDistance = 2;

    private Dictionary<Vector2Int, GameObject> activeChunks = new Dictionary<Vector2Int, GameObject>();
    private List<Vector2Int> keysToRemove = new List<Vector2Int>(); // Reusable list to avoid allocation
    private Vector2Int lastChunkCoord = new Vector2Int(-99, -99);

    private void OnEnable() => EventHub.ShipMoved.AddListener(OnPlayerMoved);
    private void OnDisable() => EventHub.ShipMoved.RemoveListener(OnPlayerMoved);

    // This method is linked via UnityEvent in the Inspector
    public void OnPlayerMoved(MovementData data) {
        Vector2Int currentCoord = new Vector2Int(
            Mathf.RoundToInt(data.Position.x / chunkSize),
            Mathf.RoundToInt(data.Position.y / chunkSize)
        );

        if (currentCoord != lastChunkCoord) {
            UpdateGrid(currentCoord);
            lastChunkCoord = currentCoord;
        }
    }

    private void UpdateGrid(Vector2Int center) {
        // 1. Mark chunks for removal
        keysToRemove.Clear();

        // Manual iteration to find out-of-bounds chunks
        // Note: Dictionary requires a bit of care with for-loops; 
        // usually, we copy keys to a list first.
        var keys = new List<Vector2Int>(activeChunks.Keys);
        for (int i = 0; i < keys.Count; i++) {
            Vector2Int coord = keys[i];
            if (Mathf.Abs(coord.x - center.x) > viewDistance || Mathf.Abs(coord.y - center.y) > viewDistance) {
                keysToRemove.Add(coord);
            }
        }

        // 2. Return out-of-bounds chunks to pool
        for (int i = 0; i < keysToRemove.Count; i++) {
            GameObject chunk = activeChunks[keysToRemove[i]];
            chunk.SetActive(false);
            activeChunks.Remove(keysToRemove[i]);
        }

        // 3. Spawn/Activate new chunks
        for (int x = -viewDistance; x <= viewDistance; x++) {
            for (int y = -viewDistance; y <= viewDistance; y++) {
                Vector2Int targetCoord = new Vector2Int(center.x + x, center.y + y);
                if (!activeChunks.ContainsKey(targetCoord)) {
                    ActivateChunkAt(targetCoord);
                }
            }
        }
    }

    private void ActivateChunkAt(Vector2Int coord) {
        GameObject chunk = chunkPool.GetObject();
        if (chunk != null) {
            chunk.transform.position = new Vector3(coord.x * chunkSize, coord.y * chunkSize, 0);
            chunk.SetActive(true);
            activeChunks.Add(coord, chunk);
        }
    }
}