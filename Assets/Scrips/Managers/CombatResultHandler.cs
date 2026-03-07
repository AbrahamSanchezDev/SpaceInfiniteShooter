using UnityEngine;
using UnityEngine.SceneManagement;

public class StrikeModeResultHandler : MonoBehaviour {
    [SerializeField] private int scrapPerEnemy = 50;
    [SerializeField] private string driftSceneName = "DriftMode_Explore";

    public void OnCombatVictory() {
        // Calculate reward based on how many enemies we were supposed to fight
        int totalReward = GlobalGameData.Instance.EnemiesToSpawn * scrapPerEnemy;

        // Store it in the 'mailbox'
        GlobalGameData.Instance.SetCombatReward(totalReward);

        // Go back
        ReturnToDrift();
    }

    public void OnCombatDefeat() {
        // No reward, and trigger the penalty we wrote earlier
        GlobalGameData.Instance.SetCombatReward(0);
        GlobalGameData.Instance.LoseResourcesOnDefeat();

        ReturnToDrift();
    }

    private void ReturnToDrift() {
        SceneManager.LoadScene(driftSceneName);
    }
}