using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager main;

    [Header("Path References")]
    public Transform startPoint;      // Where enemies spawn (I)
    public Transform endPoint;        // Where enemies exit (O) - optional use
    public Transform[] path;          // Full enemy path

    [Header("Currency")]
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
            currency -= amount;
            return true;
        }
        else
        {
            Debug.Log("Don't have the cash for it");
            return false;
        }
    }

    // 🔥 New: sets the enemy path and start/end points from MazeSpawner
    public void SetPath(Transform[] path, Transform start, Transform end)
    {
        this.path = path;
        this.startPoint = start;
        this.endPoint = end;
    }
}