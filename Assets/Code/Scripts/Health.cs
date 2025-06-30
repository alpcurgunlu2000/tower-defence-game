using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] private int hitPoints = 2; // 2 POINTS to HIT before it gets destroyed
    [SerializeField] private int currencyWorth = 50;
    private bool isDestroyed = false;
    public void TakeDamage(int dmg)
    {
        hitPoints -= dmg;

        if (hitPoints <= 0)
        {
            EnemySpawner.onEnemyDestroy.Invoke();
            // Give coins to player when enemy is killed by turret
            FindObjectOfType<CoinManager>().AddCoins(currencyWorth);
            isDestroyed = true;
            Destroy(gameObject);
        }
    }
}
