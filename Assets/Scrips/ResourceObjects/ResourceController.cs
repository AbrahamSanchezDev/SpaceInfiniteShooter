using UnityEngine;

public class ResourceController : MonoBehaviour {
    [SerializeField] private float initialHealth = 100f;
    [SerializeField] private int resourceYield = 10;
    [SerializeField] private bool giveResourcesOverTime = false;

    private ResourceModel _model;
    private ResourceView _view;

    private Vector3 originalScale;
    private float _harvestProgress;
    private int _yieldGiven;

    private void Awake() {
        _view = GetComponent<ResourceView>();
        originalScale = transform.localScale;
    }

    private void OnEnable() {
        // Reset Model when pulled from Object Pool
        _model = new ResourceModel(initialHealth, resourceYield);
        transform.localScale = originalScale;
        _harvestProgress = 0f;
        _yieldGiven = 0;
    }

    public void Siphon(float amount) {
        float damage = Mathf.Min(amount, _model.Health);
        bool isDestroyed = _model.TakeDamage(amount);

        // Update View
        float percent = Mathf.Clamp01(_model.Health / _model.MaxHealth);
        _view.UpdateVisuals(percent);

        if (giveResourcesOverTime) {
            if (_model.MaxHealth > 0f && damage > 0f) {
                _harvestProgress += _model.YieldAmount * (damage / _model.MaxHealth);
                int harvestNow = Mathf.FloorToInt(_harvestProgress) - _yieldGiven;
                if (harvestNow > 0) {
                    _yieldGiven += harvestNow;
                    EventHub.ResourceHarvested.Invoke("Scrap", harvestNow);
                }
            }
        }

        if (isDestroyed) {
            if (giveResourcesOverTime) {
                int remainder = _model.YieldAmount - _yieldGiven;
                if (remainder > 0) {
                    _yieldGiven += remainder;
                    EventHub.ResourceHarvested.Invoke("Scrap", remainder);
                }
            }
            else {
                EventHub.ResourceHarvested.Invoke("Scrap", _model.YieldAmount);
            }

            _view.OnHarvestComplete();
        }
    }
}