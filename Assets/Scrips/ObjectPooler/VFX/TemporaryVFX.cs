using UnityEngine;

public class TemporaryVFX : MonoBehaviour {
    [SerializeField] private float duration = 1.0f;
    private float _timer;

    private void OnEnable() {
        _timer = duration;
    }

    private void Update() {
        _timer -= Time.deltaTime;
        if (_timer <= 0) {
            gameObject.SetActive(false); // Return to pool
        }
    }
}