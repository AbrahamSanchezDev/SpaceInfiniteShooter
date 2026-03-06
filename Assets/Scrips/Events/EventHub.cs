using UnityEngine.Events;

public static class EventHub {
    // Global access point for the Ship's movement
    public static readonly UnityEvent<MovementData> ShipMoved = new UnityEvent<MovementData>();
    // string = ResourceName, int = Amount
    public static readonly UnityEvent<string, int> ResourceHarvested = new UnityEvent<string, int>();
}