using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class HealthBar : MonoBehaviour
{
    public static HealthBar main;

    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int _health = 100;

    [SerializeField] private Slider _healthBar;
    [SerializeField] private int _enemyDamage = 20; // Example: 20 damage = 20%
    [SerializeField] private GameOverManager gameOverManager;

    public Image fillImage; // Assign in inspector (the fill part of the bar)
    private RectTransform fillRect;
    private float fullWidth;

    private void Awake()
    {
        main = this;
        Assert.IsNotNull(_healthBar, "Health bar not set in Inspector!");
        if (fillImage != null)
            fillRect = fillImage.GetComponent<RectTransform>();
    }

    private void Start()
    {
        _health = maxHealth;

        // Get the full width of the parent (background) bar
        if (fillRect != null && fillRect.parent != null)
        {
            fullWidth = ((RectTransform)fillRect.parent).rect.width;
            SetFillWidth(1f); // Start full
        }
    }

    private void SetFillWidth(float percent)
    {
        if (fillRect != null)
        {
            fillRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, fullWidth * percent);
        }
    }

    private void UpdateHealthBar()
    {
        float percent = Mathf.Clamp01((float)_health / maxHealth);
        _healthBar.value = percent;
        //SetFillWidth(percent);
    }

    public void TakeDamage(int damage)
    {
        _health = Mathf.Max(_health - damage, 0);
        UpdateHealthBar();
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

    // Optional: Call this if you want to set health directly
    public void SetHealth(int newHealth)
    {
        _health = Mathf.Clamp(newHealth, 0, maxHealth);
        UpdateHealthBar();
    }
}