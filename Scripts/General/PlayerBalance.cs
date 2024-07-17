using System;
using UnityEngine;

public class PlayerBalance : MonoBehaviour
{
    public static PlayerBalance Instance { get; private set; }

    public Action OnBalanceChanged;
    public int Balance { get; private set; }
    public int Spins { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        Spins = PlayerPrefs.GetInt("Spins", 5);
        Balance = PlayerPrefs.GetInt("Balance", 5000);
    }
    public void ChangeBalance(int count)
    {
        Balance += count;
        OnBalanceChanged?.Invoke();
        Save();
    }
    public void ChangeSpins(int count)
    {
        Spins += count;
        OnBalanceChanged?.Invoke();
        Save();
    }
    private void Save()
    {
        PlayerPrefs.SetInt("Spins", Spins);
        PlayerPrefs.SetInt("Balance", Balance);
    }
}
