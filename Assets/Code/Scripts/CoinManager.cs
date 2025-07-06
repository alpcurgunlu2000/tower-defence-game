using UnityEngine;
using TMPro;

public class CoinManager : MonoBehaviour
{
    public int coins = 100;
    public TMP_Text coinText;

    private void Start()
    {
        UpdateCoinUI();
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        UpdateCoinUI();
    }

    public bool SpendCoins(int amount)
    {
        if (coins >= amount)
        {
            coins -= amount;
            UpdateCoinUI();
            return true;
        }
        return false;
    }

    private void UpdateCoinUI()
    {
        coinText.text = coins.ToString();
    }
}