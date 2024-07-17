using System;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    public Action OnTimeOver;
    public static GameTimer Instance { get; private set; }

    public float Timer {get; private set; }

    private bool _isTimerEnabled = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }
    private void FixedUpdate()
    {
        if (!_isTimerEnabled)
            return;
        if (Timer > 0)
        {
            Timer -= Time.fixedDeltaTime;
        }
        else
        { 
            _isTimerEnabled=false;
            OnTimeOver?.Invoke();
        }

    }
    public void StartTimer(float time)
    {
        Timer = time;
        _isTimerEnabled = true;
    }
    public void StopTimer()
    {
        _isTimerEnabled = false;
    }
    public void ContinueTimer()
    {
        _isTimerEnabled = true;
    }
}
