using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    public int CurrentLevelId { get; private set; }
    public int TipCost { get; private set; }

    [SerializeField] private Transform _levelParent;

    [SerializeField] private List<GameObject> _levelPrefabs;

    private Level _currentLevel;


    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        Initialize();
    }
    private void Initialize()
    {
        CurrentLevelId = PlayerPrefs.GetInt("LevelID", 1);
    }
    public void SpawnLevel()
    {
        if (PlayerBalance.Instance.Balance != 0)
            TipCost = (PlayerBalance.Instance.Balance / 5);
        else
            TipCost = 1000;

        ClearLevel();
        if (PlayerPrefs.GetInt("Tutorial", 0) == 1)
        {
            GameObject level;

            if (CurrentLevelId >= _levelPrefabs.Count)
                level = Instantiate(_levelPrefabs[_levelPrefabs.Count - 1], _levelParent);
            else
                level = Instantiate(_levelPrefabs[CurrentLevelId - 1], _levelParent);

            _currentLevel = level.GetComponent<Level>();
            _currentLevel.Initialize();
            FindACoupleUI.Instance.TimerAlert();
        }
        else
            FindACoupleUI.Instance.OpenTutorial();
    }
    public void ClearLevel()
    {
        if(_currentLevel != null)
            Destroy(_currentLevel.gameObject, 0.5f);
    }
    public void RestartLevel()
    {
        ClearLevel();
        SpawnLevel();
    }
    public void TakeTip()
    {
        if (PlayerBalance.Instance.Balance < TipCost)
            return;

        PlayerBalance.Instance.ChangeBalance(-TipCost);
        _currentLevel.TakeTip();
    }
    public void OnRouletteWin()
    {
        _currentLevel.OnRouletteWin();
    }
    public void OnLevelSuccess()
    {
        ClearLevel();
        CurrentLevelId++;
        Save();
    }
    public int GetSpinsReward() => _currentLevel.GetSpinsReward();
    private void Save()
    {
        PlayerPrefs.SetInt("LevelID", CurrentLevelId);
    }
}
