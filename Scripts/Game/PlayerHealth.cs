using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance { get; private set; }

    public Action OnHealthOver;

    public int Health { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }
    public void InitializeHealth(int count) => Health = count;
    private void CheckHelath()
    {
        if (Health == 0)
            OnHealthOver?.Invoke();
    }
    public void DecreaseHealth()
    {
        Health--;
        CheckHelath();
    }
    public void IncreaseHealth() => Health++;
}
