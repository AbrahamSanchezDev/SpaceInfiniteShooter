using System.Collections.Generic;
using UnityEngine;

public class WorldSpawner : MonoBehaviour {
    [SerializeField] private ObjectPooler chunkPool;
    [SerializeField] private ObjectPooler asteroidPool; // Pool for individual asteroids
    [SerializeField] private float chunkSize = 50f;
    [SerializeField] private int viewDistance = 2;
    [SerializeField] private int asteroidsPerChunk = 5;

    private Dictionary<Vector2Int, GameObject> _activeChunks = new Dictionary<Vector2Int, GameObject>();
    private List<Vector2Int> _keysBuffer = new List<Vector2Int>(); // Reusable list
    private List<Vector2Int> _removalBuffer = new List<Vector2Int>(); // Reusable list
    private Vector2Int _lastChunkCoord = new Vector2Int(-99, -99);


    private void OnEnable() {
        EventHub.ShipMoved.AddListener(OnPlayerMoved);
        EventHub.OriginResetRequested.AddListener(ShiftChunks);
    }

    private void OnDisable() {
        EventHub.ShipMoved.RemoveListener(OnPlayerMoved);
        EventHub.OriginResetRequested.RemoveListener(ShiftChunks);
    }

    private void ShiftChunks(Vector3 offset) {
        // 1. Shift the stored "lastChunkCoord" so the math stays correct
        _lastChunkCoord = new Vector2Int(
            Mathf.RoundToInt((_lastChunkCoord.x * chunkSize - offset.x) / chunkSize),
            Mathf.RoundToInt((_lastChunkCoord.y * chunkSize - offset.y) / chunkSize)
        );

        // 2. Move all active chunks in the hierarchy
        // We use a list to avoid dictionary modification errors
        foreach (var chunk in _activeChunks.Values) {
            chunk.transform.position -= offset;
        }

        // 3. Rebuild the dictionary with new coordinates
        _keysBuffer.Clear();
        foreach (var key in _activeChunks.Keys) _keysBuffer.Add(key);

        var tempDict = new Dictionary<Vector2Int, GameObject>();
        for (int i = 0; i < _keysBuffer.Count; i++) {
            Vector2Int oldKey = _keysBuffer[i];
            GameObject val = _activeChunks[oldKey];

            Vector2Int newKey = new Vector2Int(
                Mathf.RoundToInt(val.transform.position.x / chunkSize),
                Mathf.RoundToInt(val.transform.position.y / chunkSize)
            );
            tempDict.Add(newKey, val);
        }

        _activeChunks = tempDict;
    }

    public void OnPlayerMoved(MovementData data) {
        // Vector2Int is a struct, no heap allocation here
        Vector2Int currentCoord = new Vector2Int(
            Mathf.RoundToInt(data.Position.x / chunkSize),
            Mathf.RoundToInt(data.Position.y / chunkSize)
        );

        if (currentCoord != _lastChunkCoord) {
            UpdateGrid(currentCoord);
            _lastChunkCoord = currentCoord;
        }
    }

    private void UpdateGrid(Vector2Int center) {
        _removalBuffer.Clear();
        _keysBuffer.Clear();

        foreach (var pair in _activeChunks) _keysBuffer.Add(pair.Key);

        for (int i = 0; i < _keysBuffer.Count; i++) {
            Vector2Int coord = _keysBuffer[i];
            if (Mathf.Abs(coord.x - center.x) > viewDistance || Mathf.Abs(coord.y - center.y) > viewDistance) {
                _removalBuffer.Add(coord);
            }
        }

        for (int i = 0; i < _removalBuffer.Count; i++) {
            CleanUpChunk(_removalBuffer[i]);
        }

        // Spawn new ones
        for (int x = -viewDistance; x <= viewDistance; x++) {
            for (int y = -viewDistance; y <= viewDistance; y++) {
                Vector2Int targetCoord = new Vector2Int(center.x + x, center.y + y);
                if (!_activeChunks.ContainsKey(targetCoord)) {
                    ActivateChunkAt(targetCoord);
                }
            }
        }
    }

    private void ActivateChunkAt(Vector2Int coord) {
        GameObject chunk = chunkPool.GetObject();
        if (chunk == null) return;

        chunk.transform.position = new Vector3(coord.x * chunkSize, coord.y * chunkSize, 0);
        chunk.SetActive(true);
        _activeChunks.Add(coord, chunk);

        // Populate chunk with fresh asteroids from the pool
        for (int i = 0; i < asteroidsPerChunk; i++) {
            GameObject asteroid = asteroidPool.GetObject();
            if (asteroid != null) {
                asteroid.transform.SetParent(chunk.transform);
                // Random position inside the 50x50 chunk
                asteroid.transform.localPosition = new Vector3(Random.Range(-20, 20), Random.Range(-20, 20), 0);
                asteroid.SetActive(true);
            }
        }
    }

    private void CleanUpChunk(Vector2Int coord) {
        GameObject chunk = _activeChunks[coord];

        // Return all asteroids in this chunk to the asteroid pool
        // We iterate backwards when disabling/deparenting
        for (int i = chunk.transform.childCount - 1; i >= 0; i--) {
            if (chunk.transform.GetChild(i).name == "Visuals") continue;
            chunk.transform.GetChild(i).gameObject.SetActive(false);
        }

        chunk.SetActive(false);
        _activeChunks.Remove(coord);
    }
}