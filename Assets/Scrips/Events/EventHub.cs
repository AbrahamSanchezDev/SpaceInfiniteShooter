using UnityEngine.Events;
using UnityEngine;

public static class EventHub {
    // Global access point for the Ship's movement
    public static readonly UnityEvent<MovementData> ShipMoved = new UnityEvent<MovementData>();
    // string = ResourceName, int = Amount
    public static readonly UnityEvent<string, int> ResourceHarvested = new UnityEvent<string, int>();

    // New: Passes the Offset vector to everything
    public static readonly UnityEvent<Vector3> OriginResetRequested = new UnityEvent<Vector3>();

    // Combat State Events
    public static readonly UnityEvent StrikeModeStarted = new UnityEvent();
    public static readonly UnityEvent StrikeModeEnded = new UnityEvent();

    // Pass the Enemy Type or Difficulty to the Combat Scene
    public static readonly UnityEvent<int> LoadCombatScene = new UnityEvent<int>();
}