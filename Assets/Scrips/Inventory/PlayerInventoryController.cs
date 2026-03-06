using UnityEngine;

public class PlayerInventoryController : MonoBehaviour {
    private InventoryModel _model;

    private void Awake() {
        _model = new InventoryModel();
    }

    private void OnEnable() {
        // Subscribe to a global event that triggers when any resource is harvested
        EventHub.ResourceHarvested.AddListener(HandleResourceHarvested);
    }

    private void OnDisable() {
        EventHub.ResourceHarvested.RemoveListener(HandleResourceHarvested);
    }

    private void HandleResourceHarvested(string type, int amount) {
        _model.AddResource(type, amount);
        Debug.Log($"Inventory Update: {type} = {_model.GetAmount(type)}");
    }
}