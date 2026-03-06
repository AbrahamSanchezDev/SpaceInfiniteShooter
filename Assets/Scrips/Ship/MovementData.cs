using UnityEngine;

[System.Serializable]
public struct MovementData {
    public Vector3 Position;
    public Vector2 Velocity;

    public MovementData(Vector3 pos, Vector2 vel) {
        Position = pos;
        Velocity = vel;
    }
}