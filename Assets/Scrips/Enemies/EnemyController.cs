using UnityEngine;

public class EnemyController : MonoBehaviour {
    private enum AIState { Idle, Maneuver, Attack, Charge }

    [Header("AI Settings")]
    [SerializeField] private float detectionRange = 18f;
    [SerializeField] private float preferredDistance = 8f;
    [SerializeField] private float strafeSpeed = 4f;
    [SerializeField] private float chargeSpeed = 12f;
    [SerializeField] private float attackCooldown = 1.5f;

    [SerializeField] private WeaponController weapon;

    private EnemyModel _model;
    private Rigidbody2D _rb;
    private Transform _myTransform;
    private AIState _currentState = AIState.Idle;

    private Vector3 _lastKnownPlayerPos;
    private float _nextActionTime;
    private int _shotsFiredInBurst;
    private int _targetBurstCount;
    private bool _playerInRange;

    [Header("Death Settings")]
    [SerializeField] private int Health = 40;
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private int scrapDropAmount = 25;
    [SerializeField] private string resourceType = "Scrap";

    private void Awake() {
        _myTransform = transform;
        _rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable() {
        _model = new EnemyModel(Health, strafeSpeed, 100);
        _model.OnDeath += HandleDeath;

        _currentState = AIState.Idle;
        _targetBurstCount = Random.Range(3, 6); // 3 to 5 shots
        _shotsFiredInBurst = 0;
        EventHub.ShipMoved.AddListener(OnPlayerMoved);
    }

    private void OnDisable() {
        EventHub.ShipMoved.RemoveListener(OnPlayerMoved);
        _model.OnDeath -= HandleDeath;
    }

    private void OnPlayerMoved(MovementData data) {
        _lastKnownPlayerPos = data.Position;
        _playerInRange = Vector3.Distance(_myTransform.position, _lastKnownPlayerPos) < detectionRange;
    }

    private void FixedUpdate() {
        if (!_playerInRange && _currentState != AIState.Charge) {
            _currentState = AIState.Idle;
            _rb.linearVelocity = Vector2.Lerp(_rb.linearVelocity, Vector2.zero, 0.1f);
            return;
        }

        ExecuteStateMachine();
    }

    private void ExecuteStateMachine() {
        LookAt(_lastKnownPlayerPos);

        switch (_currentState) {
            case AIState.Idle:
                if (_playerInRange) _currentState = AIState.Maneuver;
                break;

            case AIState.Maneuver:
                ManeuverBehavior();
                // If cooldown passed and we haven't finished burst, attack
                if (Time.time >= _nextActionTime) _currentState = AIState.Attack;
                break;

            case AIState.Attack:
                AttackBehavior();
                break;

            case AIState.Charge:
                ChargeBehavior();
                break;
        }
    }

    private void AttackBehavior() {
        _rb.linearVelocity = Vector2.Lerp(_rb.linearVelocity, Vector2.zero, 0.2f);

        // Fire Projectile
        //Debug.Log($"<color=orange>Enemy Fire ({_shotsFiredInBurst + 1}/{_targetBurstCount})</color>");
        _shotsFiredInBurst++;
        _nextActionTime = Time.time + attackCooldown;

        weapon.RequestFire();

        if (_shotsFiredInBurst >= _targetBurstCount) {
            _currentState = AIState.Charge;
            // Visual Telegraph for Charge
            Debug.Log("<color=red>ENEMY CHARGING!</color>");
        }
        else {
            _currentState = AIState.Maneuver;
        }
    }

    private void ChargeBehavior() {
        // Move directly at player with high speed
        Vector2 dir = (_lastKnownPlayerPos - _myTransform.position).normalized;
        _rb.linearVelocity = dir * chargeSpeed;

        // Force Strike Mode if close enough during a charge
        if (Vector3.Distance(_myTransform.position, _lastKnownPlayerPos) < 4f) {
            // This ensures the radar/camera definitely kicks into combat mode
            EventHub.StrikeModeStarted.Invoke();
        }

        // Exit Charge if we overshoot or hit something
        if (Vector3.Distance(_myTransform.position, _lastKnownPlayerPos) < 1f) {
            _shotsFiredInBurst = 0;
            _targetBurstCount = Random.Range(3, 6);
            _currentState = AIState.Maneuver;
            _nextActionTime = Time.time + attackCooldown;
        }
    }

    private void ManeuverBehavior() {
        Vector2 toPlayer = (_lastKnownPlayerPos - _myTransform.position).normalized;
        Vector2 strafeDir = new Vector2(-toPlayer.y, toPlayer.x);

        float dist = Vector3.Distance(_myTransform.position, _lastKnownPlayerPos);
        Vector2 moveDir = strafeDir;
        if (dist > preferredDistance + 1f) moveDir += toPlayer;
        else if (dist < preferredDistance - 1f) moveDir -= toPlayer;

        _rb.linearVelocity = moveDir.normalized * _model.Speed;
    }

    private void LookAt(Vector3 target) {
        Vector2 dir = (target - _myTransform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        _rb.MoveRotation(Mathf.LerpAngle(_rb.rotation, angle, 0.15f));
    }


    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            // This triggers the switch to the MinimalShooting scene
            EventHub.StrikeModeStarted.Invoke();
        }
    }



    #region Battle

    public void TakeDamage(float amount, Vector3 hitPoint) {
        _model.TakeDamage(amount);
        // Visual feedback
        VFXManager.Instance.SpawnHitSpark(hitPoint); 
    }

    private void HandleDeath() {
        VFXManager.Instance.SpawnExplosion(transform.position);
        EventHub.ResourceHarvested.Invoke(resourceType, scrapDropAmount);
        gameObject.SetActive(false);
    }

    #endregion
}