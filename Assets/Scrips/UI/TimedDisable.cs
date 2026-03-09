using UnityEngine;
using System.Collections;

public class TimedDisable : MonoBehaviour {
    [Tooltip("Time in seconds before the object is disabled.")]
    public float disableTime = 3.0f;

    private float _timer;

    private IEnumerator Start() {
        yield return new WaitForSeconds(disableTime);
        gameObject.SetActive(false);
    }
}