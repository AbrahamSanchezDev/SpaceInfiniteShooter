using UnityEngine;

public class ResourceController : MonoBehaviour {
    [SerializeField] private float initialHealth = 100f;
    [SerializeField] private int resourceYield = 10;

    private ResourceModel _model;
    private ResourceView _view;

    private Vector3 originalScale;

    private void Awake() {
        _view = GetComponent<ResourceView>();
        originalScale = transform.localScale;
    }

    private void OnEnable() {
        // Reset Model when pulled from Object Pool
        _model = new ResourceModel(initialHealth, resourceYield);
        transform.localScale = originalScale;
    }

    public void Siphon(float amount) {
        bool isDestroyed = _model.TakeDamage(amount);

        // Update View
        float percent = _model.Health / _model.MaxHealth;
        _view.UpdateVisuals(percent);

        if (isDestroyed) {
            // Broadcast the type and amount defined in this asteroid's config
            EventHub.ResourceHarvested.Invoke("Scrap", _model.YieldAmount);
            _view.OnHarvestComplete();
        }
    }
}