using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace MinimalShooting {
    public class PlayManager : SingletonMonobehaviour<PlayManager> {
        [SerializeField]
        Camera gameCamera;

        public int CurrentLives = 1;
        public UnityEvent OnLostALive = new UnityEvent();

        public Action DoCustomReturnToMainScene;
        public void GameOver() {
            this.gameCamera.GetComponent<CameraShake>().enabled = true;
            StartCoroutine(ReloadCurrentScene());
        }


        IEnumerator ReloadCurrentScene() {
            yield return new WaitForSeconds(3.0f);
            CurrentLives--;
            if (CurrentLives > 0) {
                OnLostALive?.Invoke();
                ReloadScene();
            }
            else {
                LoadMainScene();
            }

        }

        public void ReloadScene() {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void LoadMainScene() {
            if (DoCustomReturnToMainScene != null) {
                DoCustomReturnToMainScene();
                return;
            }
            SceneManager.LoadScene(0);
        }
    }
}
