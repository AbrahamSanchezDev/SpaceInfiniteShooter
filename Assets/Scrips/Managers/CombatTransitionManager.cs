using UnityEngine;
using UnityEngine.SceneManagement;

public class CombatTransitionManager : MonoBehaviour {
    [SerializeField] private string combatSceneName = "StrikeMode_Arena";
    [SerializeField] private int mintEnemyCount = 1;
    [SerializeField] private int maxEnemyCount = 3;

    private void OnEnable() => EventHub.StrikeModeStarted.AddListener(InitiateCombat);
    private void OnDisable() => EventHub.StrikeModeStarted.RemoveListener(InitiateCombat);

    private void InitiateCombat() {
        // 1. Save current position/state if necessary

        // Tell the global data how many enemies we encountered
        GlobalGameData.Instance.PrepareCombat(Random.Range(mintEnemyCount, maxEnemyCount));
        // Load the combat scene
        SceneManager.LoadScene(combatSceneName);
    }
}