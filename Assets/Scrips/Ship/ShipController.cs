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

    [SerializeField] private float threshold = 1000f;

    [SerializeField] private WeaponController weapon;


    [Header("Boost Settings")]
    [SerializeField] private float boostMultiplier = 2f;
    [SerializeField] private float plasmaConsumptionRate = 15f;
    [SerializeField]
    private bool _isBoosting;
    [SerializeField]
    private bool _hasPlasma;
    private void Awake() {
        myTransform = transform;
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
    }
    private void OnEnable() => EventHub.OriginResetRequested.AddListener(ResetPosition);
    private void OnDisable() => EventHub.OriginResetRequested.RemoveListener(ResetPosition);

    private void ResetPosition(Vector3 offset) {
        // Subtract the offset to bring the ship back to (0,0,0)
        myTransform.position -= offset;

        // Crucial: Update the physics velocity/position immediately so there's no "phantom force"
        rb.position -= (Vector2)offset;
    }
    private void FixedUpdate() {
        HandleMovement();
    }
    private void Update() {
        movementData.Position = myTransform.position;
        movementData.Velocity = rb.linearVelocity;

        // Check if player is holding the boost button and has plasma
        _isBoosting = input.IsBoosting;
        _hasPlasma = GlobalGameData.Instance.Plasma > 1;

        if (_isBoosting) {
            GlobalGameData.Instance.UsePlasma(plasmaConsumptionRate * Time.deltaTime);
        }

        // Broadcast movement to the Camera/World Spawner via EventHub
        EventHub.ShipMoved.Invoke(movementData);

        // The InputReader is already updated to read the "Fire" action
        if (input.IsFiring) {
            weapon.RequestFire();
        }
    }
    private void LateUpdate() {
        // magnitude is slightly expensive (sqrt), but at 1000 units it's fine. 
        // For extreme performance use sqrMagnitude > (threshold * threshold)
        if (myTransform.position.magnitude > threshold) {
            Vector3 offset = myTransform.position;
            EventHub.OriginResetRequested.Invoke(offset);
        }
    }

    private void HandleMovement() {
        float currentThrust = thrustForce;
        float currentMaxVel = maxVelocity;

        if (_isBoosting && _hasPlasma) {
            currentThrust *= boostMultiplier;
            currentMaxVel *= boostMultiplier;
        }

        // Rotation (using X input)
        float rotation = -input.MoveValue.x * rotationSpeed * Time.fixedDeltaTime;
        rb.MoveRotation(rb.rotation + rotation);

        // Thrust (using Y input)
        if (input.MoveValue.y > 0) {
            rb.AddForce(myTransform.up * input.MoveValue.y * currentThrust);
        }

        // Cap Velocity using currentMaxVel
        if (rb.linearVelocity.magnitude > currentMaxVel) {
            rb.linearVelocity = rb.linearVelocity.normalized * currentMaxVel;
        }
    }
}