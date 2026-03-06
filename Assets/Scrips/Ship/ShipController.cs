using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ShipController : MonoBehaviour {
    [SerializeField] private InputReader input;

    [Header("Movement Stats")]
    public float thrustForce = 10f;
    public float rotationSpeed = 180f;
    public float maxVelocity = 15f;

    private Rigidbody2D rb;
    private MovementData movementData = new MovementData();
    private Transform myTransform;

    private void Awake() {
        myTransform = transform;
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
    }

    private void FixedUpdate() {
        HandleMovement();
    }
    private void Update() {
        movementData.Position = myTransform.position;
        movementData.Velocity = rb.linearVelocity;

        // Broadcast movement to the Camera/World Spawner via EventHub
        EventHub.ShipMoved.Invoke(movementData);
    }

    private void HandleMovement() {
        // Rotation (using X input)
        float rotation = -input.MoveValue.x * rotationSpeed * Time.fixedDeltaTime;
        rb.MoveRotation(rb.rotation + rotation);

        // Thrust (using Y input)
        if (input.MoveValue.y > 0) {
            rb.AddForce(myTransform.up * input.MoveValue.y * thrustForce);
        }

        // Cap Velocity
        if (rb.linearVelocity.magnitude > maxVelocity) {
            rb.linearVelocity = rb.linearVelocity.normalized * maxVelocity;
        }
    }
}