public class EnemyModel {
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
    }

    public bool IsDead => Health <= 0;
}