using UnityEngine;
using UnityEngine.SceneManagement;

public class CombatTransitionManager : MonoBehaviour {
    [SerializeField] private string combatSceneName = "StrikeMode_Arena";

    private void OnEnable() => EventHub.StrikeModeStarted.AddListener(InitiateCombat);
    private void OnDisable() => EventHub.StrikeModeStarted.RemoveListener(InitiateCombat);

    private void InitiateCombat() {
        // 1. Save current position/state if necessary
        // 2. Load the combat scene additively or solo
        SceneManager.LoadScene(combatSceneName);
    }
}