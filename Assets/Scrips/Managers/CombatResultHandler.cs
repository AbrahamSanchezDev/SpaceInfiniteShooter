using MinimalShooting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StrikeModeResultHandler : MonoBehaviour {
    [SerializeField] private int scrapPerEnemy = 50;
    [SerializeField] private string driftSceneName = "DriftMode_Explore";


    [SerializeField]
    private MinimalShooting.PlayManager playManager;
    [SerializeField]
    private MinimalShooting.EnemySpawner spawner;

    private void Awake() {
        if (GlobalGameData.Instance) {
            playManager.CurrentLives = GlobalGameData.Instance.Lives;
            spawner?.SetMaxEnemies(GlobalGameData.Instance.EnemiesToSpawn);
        }
    }
    private void OnEnable() {
        if (playManager) {
            playManager.DoCustomReturnToMainScene += OnCombatDefeat;
            playManager.OnLostALive.AddListener(OnLoseLive);
        }
        spawner?.OnFinishedWaves.AddListener(OnCombatVictory);
    }
    private void OnDisable() {
        if (playManager) {
            playManager.DoCustomReturnToMainScene -= OnCombatDefeat;
            playManager.OnLostALive.AddListener(OnLoseLive);
        }
        spawner?.OnFinishedWaves.RemoveListener(OnCombatVictory);
    }
    private void OnLoseLive() {
        GlobalGameData.Instance.Lives--;
    }

    public void OnCombatVictory() {
        Debug.LogWarning("OnCombatVictory");
        // Calculate reward based on how many enemies we were supposed to fight
        int totalReward = GlobalGameData.Instance.EnemiesToSpawn * scrapPerEnemy;

        // Store it in the 'mailbox'
        GlobalGameData.Instance.SetCombatReward(totalReward);

        // Go back
        ReturnToDrift();
    }

    public void OnCombatDefeat() {
        Debug.LogWarning("OnCombatDefeat");
        // No reward, and trigger the penalty we wrote earlier
        GlobalGameData.Instance.SetCombatReward(0);
        GlobalGameData.Instance.LoseResourcesOnDefeat();

        ReturnToDrift();
    }

    private void ReturnToDrift() {
        SceneManager.LoadScene(driftSceneName);
    }
}