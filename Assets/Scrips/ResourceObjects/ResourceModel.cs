public class ResourceModel {
    public float Health { get; private set; }
    public float MaxHealth { get; private set; }
    public int YieldAmount { get; private set; }

    public ResourceModel(float health, int yield) {
        MaxHealth = health;
        Health = health;
        YieldAmount = yield;
    }

    public bool TakeDamage(float amount) {
        Health -= amount;
        return Health <= 0;
    }
}