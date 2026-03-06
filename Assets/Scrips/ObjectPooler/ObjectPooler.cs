using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour {
    public GameObject prefab;
    public int poolSize = 20;

    private List<GameObject> pool;

    void Awake() {
        pool = new List<GameObject>(poolSize);
        for (int i = 0; i < poolSize; i++) {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            pool.Add(obj);
        }
    }

    public GameObject GetObject() {
        // Using for-loop instead of foreach for performance/SOLID consistency
        for (int i = 0; i < pool.Count; i++) {
            if (!pool[i].activeInHierarchy) return pool[i];
        }
        return null; // Or expand the pool
    }
}