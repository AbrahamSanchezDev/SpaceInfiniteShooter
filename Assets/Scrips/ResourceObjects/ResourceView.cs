using UnityEngine;

public class ResourceView : MonoBehaviour {
    [SerializeField] private ParticleSystem harvestParticles;
    [SerializeField] private Transform VisualGraphic;

    private Vector3 originalScale;
    private void Awake() {
        originalScale = VisualGraphic.localScale;
    }

    public void UpdateVisuals(float healthPercent) {
        // Shrink the asteroid as it's being harvested
        VisualGraphic.localScale = originalScale * healthPercent;

        if (harvestParticles && !harvestParticles.isPlaying)
            harvestParticles.Play();
    }

    public void OnHarvestComplete() {
        // Play explosion or "poof" effect
        gameObject.SetActive(false); // Return to pool
    }
}