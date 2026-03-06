using UnityEngine;
using UnityEngine.SceneManagement;

public class StrikeModeManager : MonoBehaviour {
    public void OnPlayerDied() {
        GlobalGameData.Instance.Lives--;

        if (GlobalGameData.Instance.Lives <= 0) {
            Debug.Log("DEFEATED: Losing resources...");
            GlobalGameData.Instance.LoseResourcesOnDefeat();
            ReturnToDrift();
        }
        else {
            // Restart the fight or lose a life but stay in Strike Mode
            Debug.Log($"Life lost. Remaining: {GlobalGameData.Instance.Lives}");
        }
    }

    public void OnVictory() {
        Debug.Log("VICTORY: Returning to exploration.");
        ReturnToDrift();
    }

    private void ReturnToDrift() {
        SceneManager.LoadScene("DriftMode_Explore");
        EventHub.StrikeModeEnded.Invoke();
    }
}