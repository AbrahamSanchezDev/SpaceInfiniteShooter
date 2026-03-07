using UnityEngine;
using System;

public class GlobalGameData : MonoBehaviour {
    public static GlobalGameData Instance { get; private set; }

    // Events to notify the UI
    public event Action<int> OnLivesChanged;
    public event Action<int> OnScrapChanged;
    public event Action<float> OnPlasmaChanged;

    [SerializeField] private int _lives = 3;
    [SerializeField] private int _scrap = 0;
    [SerializeField] private float _plasma = 100f;

    [Header("Session Data")]
    public int EnemiesToSpawn = 0;

    [Header("Rewards")]
    public int PendingScrapReward = 0;

    private int maxLives = 3;
    // Properties with backing fields to trigger events
    public int Lives {
        get => _lives;
        set {
            _lives = value;
            if (_lives < 0) {
                _lives = 0;
            }
            OnLivesChanged?.Invoke(_lives);
        }
    }
    public int Scrap {
        get => _scrap;
        set {
            _scrap = value;
            if (_scrap < 0) {
                _scrap = 0;
            }
            OnScrapChanged?.Invoke(_scrap);
        }
    }
    public float Plasma {
        get => _plasma;
        set {
            _plasma = value;
            if (_plasma < 0) {
                _plasma = 0;
            }
            OnPlasmaChanged?.Invoke(_plasma);
        }
    }

    private void Awake() {
        SetInstance();
    }

    private void Update() {
        // Slowly recover 1% per second if drifting
        RecoverPlasma(1f * Time.deltaTime);
    }

    public static GlobalGameData FindInstance() {
        return FindFirstObjectByType<GlobalGameData>();
    }
    private void SetInstance() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); }
    }

    public void LoseResourcesOnDefeat() {
        Scrap = Mathf.FloorToInt(Scrap * 0.5f);
        Lives = maxLives;
    }

    public void PrepareCombat(int enemyCount) {
        EnemiesToSpawn = enemyCount;
        Lives = maxLives;
    }

    public void SetCombatReward(int amount) {
        PendingScrapReward = amount;
    }
    public void ClaimRewards() {
        if (PendingScrapReward > 0) {
            Scrap += PendingScrapReward;
            Debug.Log($"<color=green>Rewards Claimed: {PendingScrapReward} Scrap added!</color>");
            PendingScrapReward = 0; // Clear the mailbox
        }
    }


    public void UsePlasma(float amount) {
        Plasma = Mathf.Max(0, Plasma - amount);
    }

    public void RecoverPlasma(float amount) {
        Plasma = Mathf.Min(100f, Plasma + amount);
    }
}