using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager main;
    public Transform startPoint;
    public Transform[] path; // Enemy Path that the Enemy can take

    public int currency;
    private void Awake()
    {
        main = this;
    }

    private void Start()
    {
        currency = 100;
    }

    public void IncreaseCurrency(int amount)
    {
        currency += amount;
    }

        public bool SpendCurrency(int amount)
    {
        if (amount <= currency)
        {
            // BUY ITEM
            return true;
        }
        else
        {
            Debug.Log("Don't have the cash for it");
            return false;
        }
    }

}
