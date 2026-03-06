using UnityEngine;

public class GlobalGameData : MonoBehaviour {
    public static GlobalGameData Instance { get; private set; }

    [Header("Persistent Stats")]
    public int Lives = 3;
    public int Scrap = 0;
    public float Plasma = 100f;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }

    public void LoseResourcesOnDefeat() {
        // Penalty: Lose 50% of scrap on death
        Scrap = Mathf.FloorToInt(Scrap * 0.5f);
        Lives = 3; // Reset lives for the next run
    }
}