using UnityEngine;
using TMPro;
using System;

public class HUDView : MonoBehaviour {
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI livesText;
    [SerializeField] private TextMeshProUGUI scrapText;
    [SerializeField] private TextMeshProUGUI plasmaText;

    [SerializeField] private AudioClip OnScrapAdded;
    private AudioSource audioSource;
    private void Awake() {
        if (OnScrapAdded) {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null) {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
            audioSource.playOnAwake = false;
            audioSource.clip = OnScrapAdded;
            audioSource.loop = false;
        }
    }
    private void OnEnable() {
        // Subscribe to Model events
        var globalData = GlobalGameData.Instance;
        if (globalData == null) {
            globalData = GlobalGameData.FindInstance();
        }
        if (globalData == null) {
            Debug.LogWarning("NO INSTANCE OF GLOBALGAMEDATA!");
            return;
        }
        globalData.OnLivesChanged += UpdateLivesUI;
        globalData.OnScrapChanged += UpdateScrapUI;
        globalData.OnPlasmaChanged += UpdatePlasmaUI;

        // Initialize display
        UpdateLivesUI(globalData.Lives);
        UpdateScrapUI(globalData.Scrap);
        UpdatePlasmaUI(globalData.Plasma);
    }

    private void OnDisable() {
        // Unsubscribe to avoid memory leaks
        if (GlobalGameData.Instance != null) {
            GlobalGameData.Instance.OnLivesChanged -= UpdateLivesUI;
            GlobalGameData.Instance.OnScrapChanged -= UpdateScrapUI;
            GlobalGameData.Instance.OnPlasmaChanged -= UpdatePlasmaUI;
        }
    }

    private void UpdateLivesUI(int val) {
        if (livesText)
            livesText.text = $"LIVES: {val}";
    }

    private void UpdateScrapUI(int val) {
        scrapText.text = $"SCRAP: {val}";
        if (audioSource) {
            audioSource.Play();
        }
    }

    private void UpdatePlasmaUI(float val) {
        plasmaText.text = $"PLASMA: {val:F0}%";
    }
}