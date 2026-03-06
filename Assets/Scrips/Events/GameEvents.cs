using System;
using UnityEngine;

public static class GameEvents {
    // Now sends (Position, Velocity)
    public static Action<Vector3, Vector2> OnPlayerMoved;

    public static void RaisePlayerMoved(Vector3 position, Vector2 velocity) {
        OnPlayerMoved?.Invoke(position, velocity);
    }
}