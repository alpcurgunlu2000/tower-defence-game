using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class HealthBar : MonoBehaviour
{
    public static HealthBar main;

    [Range(0, 100)]
    [SerializeField] private int _health = 100;

    [SerializeField] private Slider _healthBar;

    [SerializeField] private int _enemyDamage = 50; // Damage when enemy reaches end

    [SerializeField] private GameOverManager gameOverManager;

    private void Awake()
    {
        main = this;
        Assert.IsNotNull(_healthBar, "Health bar not set in Inspector!");
    }

    private void Start()
    {
        _healthBar.value = _health;
    }

    public void TakeDamage(int damage)
    {
        _health = Mathf.Max(_health - damage, 0);
        _healthBar.value = _health;
        Debug.Log($"Player taking damage: {damage}, new health: {_health}");
        
        if (_health <= 0)
        {
            GameOver();
        }
    }

    public void EnemyReachedEnd()
    {
        TakeDamage(_enemyDamage);
    }

    private void GameOver()
    {
        Debug.Log("Game Over! Player health reached 0");
        gameOverManager.TriggerGameOver();
    }
}