using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Level : MonoBehaviour
{
    [Header("LevelSettings")]
    [Space]
    [SerializeField] private bool _randomLevel;
    [SerializeField] private int _rewardCount;
    [SerializeField] private int _rewardSpinsCount;
    [SerializeField] private int _itemsCount = 36;
    [SerializeField] private int _gridColumns;
    [SerializeField] private float _levelTime;
    [SerializeField] private int _levelHealth;

    [Space]
    [Header("Other")]
    [SerializeField] private Transform _itemsParent;

    [SerializeField] private GridLayoutGroup _layoutGroup;

    [SerializeField] private GameObject _itemPrefab;

    private List<Item> _items = new List<Item>();
    private List<int> _itemPos;
    private Item _lastItem;

    private int _foundCouples;

    private void OnEnable()
    {
        GameTimer.Instance.OnTimeOver += GameOver;
        PlayerHealth.Instance.OnHealthOver += GameOver;
    }
    private void OnDisable()
    {
        GameTimer.Instance.OnTimeOver -= GameOver;
        PlayerHealth.Instance.OnHealthOver -= GameOver;
    }
    public void Initialize()
    {
        if (!_randomLevel)
        {
            for (int i = 0; i < _itemsParent.childCount; i++)
            {
                Item itemComponent = _itemsParent.GetChild(i).GetComponent<Item>();
                if (itemComponent != null)
                    _items.Add(itemComponent);
            }
            float delay = 1;
            foreach (var item in _items)
            {
                delay += 0.1f;
                item.Initialize(this, delay);
            }
            _itemsCount = _items.Count;
        }
        else
            GenerateLevel();
        transform.DOScale(transform.localScale, 1f).SetLink(gameObject).OnKill(() => { _layoutGroup.enabled = false; });

        PlayerHealth.Instance.InitializeHealth(_levelHealth + (UpgradeSystem.Instance.Upgrades[3]*2));

        GameTimer.Instance.StartTimer(_levelTime + (UpgradeSystem.Instance.Upgrades[2]*5));

        FindACoupleUI.Instance.UpdateBalance();
        FindACoupleUI.Instance.UpdateRewardCount(_rewardCount);
        FindACoupleUI.Instance.UpdateLevelNumber(LevelManager.Instance.CurrentLevelId);
        FindACoupleUI.Instance.UpdateTimerAlert();
        FindACoupleUI.Instance.UpdateHealth();
        FindACoupleUI.Instance.UpdateCouples(_foundCouples, _itemsCount / 2);
    }
    private void GenerateLevel()
    {
        _layoutGroup.constraintCount = _gridColumns;

        _itemPos = new List<int>();
        float delay = 1;
        for (int i = 0; i < _itemsCount/2; i++)
        {
            int randomType = UnityEngine.Random.Range(0, Enum.GetValues(typeof(ElementType)).Length-1);

            delay += 0.05f;

            var item = Instantiate(_itemPrefab, _itemsParent).GetComponent<Item>();
            var secondItem = Instantiate(_itemPrefab, _itemsParent).GetComponent<Item>();
            
            item.Initialize(this, delay, (ElementType)randomType);
            secondItem.Initialize(this, delay, (ElementType)randomType);

            _items.Add(item);
            _items.Add(secondItem);
        }
        for (int i = 0; i < _items.Count; i++)
            _itemPos.Add(i);
        for (int i = 0; i < _items.Count; i++)
        {
            int randomNumber = UnityEngine.Random.Range(0, _itemPos.Count);
            _items[i].transform.SetSiblingIndex(_itemPos[randomNumber]);
            _itemPos.Remove(_itemPos[randomNumber]);
        }
    }
    public void TakeTip()
    {
        Item item;
        if (_lastItem == null)
            item = _items[UnityEngine.Random.Range(0, _items.Count)];
        else
            item = _lastItem;

        Item secondItem = null;
        ElementType itemType = item.Type;

        for (int i = 0; i < _items.Count; i++)
        {
            if (item != _items[i])
            {
                if (_items[i].Type == itemType)
                { 
                    secondItem = _items[i];
                    break;
                }
            }
        }
        item.Open();
        secondItem.Open();
    }
    public void OnRouletteWin()
    {
        GameTimer.Instance.StartTimer(_levelTime + (UpgradeSystem.Instance.Upgrades[2] * 5));
        PlayerHealth.Instance.InitializeHealth(_levelHealth + (UpgradeSystem.Instance.Upgrades[3] * 2));
        FindACoupleUI.Instance.UpdateHealth();
    }
    public void OnItemOpened(Item item)
    {
        if (_lastItem != null)
        {
            if (_lastItem.Type == item.Type)
            {
                item.OnItemCorrect();
                _lastItem.OnItemCorrect();
                FindACoupleUI.Instance.OnCorrectCouple();
                FindACoupleUI.Instance.OnCardsChanged(1.5f);

                _foundCouples++;

                AudioSystem.Instance.PlaySound(AudioSystem.Instance.CardCorrect, 1f);
                CardsSystem.Instance.OnCardsCollected(item.Type, 2);

                _items.Remove(item);
                _items.Remove(_lastItem);
            }
            else
            {
                item.Close(0.8f);
                _lastItem.Close(0.8f);

                AudioSystem.Instance.PlaySound(AudioSystem.Instance.CardIncorrect, 1f);
                PlayerHealth.Instance.DecreaseHealth();
                FindACoupleUI.Instance.DecreaseHealth();
            }
            _lastItem = null;
        }
        else
            _lastItem = item;

        if (_items.Count == 0)
            GameSuccess();

        FindACoupleUI.Instance.UpdateCouples(_foundCouples, _itemsCount / 2);
    }
    private void GameOver()
    {
        AudioSystem.Instance.PlaySound(AudioSystem.Instance.LevelFailed, 1f);
        FindACoupleUI.Instance.OnGameOver();
        GameTimer.Instance.StopTimer();
    }
    private void GameSuccess()
    {
        AudioSystem.Instance.PlaySound(AudioSystem.Instance.LevelSuccess, 1f);
        LevelManager.Instance.OnLevelSuccess();
        PlayerBalance.Instance.ChangeBalance(_rewardCount);
        PlayerBalance.Instance.ChangeSpins(_rewardSpinsCount);
        FindACoupleUI.Instance.OnGameSuccess();
        GameTimer.Instance.StopTimer();
    }
    public int GetReward() => _rewardCount;
    public int GetSpinsReward() => _rewardSpinsCount;
}
