using System;
using System.Collections.Generic;

public class InventoryModel {
    // Key: Resource Type (e.g., "Scrap", "Plasma"), Value: Amount
    private Dictionary<string, float> _resources = new Dictionary<string, float>();

    // Event to notify the View when data changes
    public event Action<string, float> OnResourceChanged;

    public void AddResource(string type, float amount) {
        if (!_resources.ContainsKey(type))
            _resources[type] = 0;

        _resources[type] += amount;
        OnResourceChanged?.Invoke(type, _resources[type]);
    }

    public float GetAmount(string type) {
        return _resources.ContainsKey(type) ? _resources[type] : 0;
    }
}