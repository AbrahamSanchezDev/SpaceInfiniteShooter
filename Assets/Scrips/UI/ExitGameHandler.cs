using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ExitGameHandler : MonoBehaviour {
    private Button _exitButton;

    private void Awake() {
        // Automatically get the Button component attached to this GameObject
        _exitButton = GetComponent<Button>();

        // Add the listener via code instead of the Inspector
        _exitButton.onClick.AddListener(QuitApplication);
    }

    private void QuitApplication() {
        Debug.Log("Exiting Application...");

#if UNITY_WEBGL
        // For WebGL builds, we can't quit the application, so we can redirect to a different page or simply log a message
        Debug.Log("Cannot quit application in WebGL. Please close the browser tab.");
        // Reload the page
        Application.ExternalEval("document.location.reload(true);");
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
#endif

        // This works for standalone builds (.exe, .app)
        Application.Quit();

        // This allows the exit button to work while testing inside the Unity Editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void OnDestroy() {
        // Clean up the listener when the object is destroyed to prevent memory leaks
        if (_exitButton != null) {
            _exitButton.onClick.RemoveListener(QuitApplication);
        }
    }
}