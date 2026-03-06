using UnityEngine.Events;
using UnityEngine;

public static class EventHub {
    // Global access point for the Ship's movement
    public static readonly UnityEvent<MovementData> ShipMoved = new UnityEvent<MovementData>();
    // string = ResourceName, int = Amount
    public static readonly UnityEvent<string, int> ResourceHarvested = new UnityEvent<string, int>();

    // New: Passes the Offset vector to everything
    public static readonly UnityEvent<Vector3> OriginResetRequested = new UnityEvent<Vector3>();
}