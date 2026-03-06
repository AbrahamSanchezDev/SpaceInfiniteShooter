using UnityEngine;
using UnityEngine.SceneManagement;

public class CombatResultHandler : MonoBehaviour {
    public void OnEnemiesCleared() {
        // Add resources earned in combat to the model here
        SceneManager.LoadScene("DriftMode_Explore");
    }
}