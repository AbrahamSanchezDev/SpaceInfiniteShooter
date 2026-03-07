using UnityEngine;

public class PlayerInventoryController : MonoBehaviour {
    private InventoryModel _model;

    private void Awake() {
        _model = new InventoryModel();
    }

    private void Start() {
        // Check if we just came back from a successful combat
        if (GlobalGameData.Instance != null) {
            GlobalGameData.Instance.ClaimRewards();
        }
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

        if (type == "Scrap") {
            if (GlobalGameData.Instance)
                GlobalGameData.Instance.Scrap += amount;
        }
        if (type == "Plasma") {
            if (GlobalGameData.Instance)
                GlobalGameData.Instance.RecoverPlasma(amount);
        }
        if (type == "Lives") {
            if (GlobalGameData.Instance)
                GlobalGameData.Instance.Lives += amount;
        }
    }
}