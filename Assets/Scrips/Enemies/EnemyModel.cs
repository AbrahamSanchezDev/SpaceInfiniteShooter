public class EnemyModel {
    public event System.Action OnDeath;

    public float Health { get; private set; }
    public float Speed { get; private set; }
    public int ScoreValue { get; private set; }

    public EnemyModel(float health, float speed, int score) {
        Health = health;
        Speed = speed;
        ScoreValue = score;
    }

    public void TakeDamage(float amount) {
        Health -= amount;
        if (IsDead) OnDeath?.Invoke();
    }

    public bool IsDead => Health <= 0;
}