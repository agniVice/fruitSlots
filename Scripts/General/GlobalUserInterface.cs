using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class GlobalUserInterface : MonoBehaviour
{
    public static GlobalUserInterface Instance { get; private set; }

    public Action OnAnimOnMid;

    [Header("UI`s")]

    [SerializeField] private GameObject _gamePanel;
    [SerializeField] private GameObject _menuPanel;

    [SerializeField] private GameObject _findACouplePanel;
    [SerializeField] private GameObject _slotPanel;

    [Space]
    [Header("FadeFX")]
    [SerializeField] private Image _fade;

    [Space]
    [SerializeField] private float _timeFade;

    [Space]
    [SerializeField] private Transform _bottomPosition;
    [SerializeField] private Transform _upperPosition;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        _fade.transform.position = _bottomPosition.position;
    }
    private void Start()
    {
        OpenMenu();
    }
    public void OpenGame(bool isOpenSlot = false)
    {
        _fade.transform.position = _bottomPosition.position;
        _fade.transform.DOMove(_upperPosition.position, _timeFade).SetLink(_fade.gameObject).SetEase(Ease.Linear);
        _fade.DOFade(1, _timeFade/2f).SetLink(_fade.gameObject).SetEase(Ease.Linear).OnKill(() => {

            _slotPanel.SetActive(false);
            _findACouplePanel.SetActive(false);

            if (isOpenSlot == false) OpenFindACouple();
            else OpenSlot();

            _menuPanel.SetActive(false);
            _gamePanel.SetActive(true);
        });
    }
    public void OpenFindACouple()
    {
        _findACouplePanel.SetActive(true);
        LevelManager.Instance.SpawnLevel();
    }
    public void OpenMenu()
    {
        _fade.transform.position = _upperPosition.position;
        _fade.transform.DOMove(_bottomPosition.position, _timeFade).SetLink(_fade.gameObject).SetEase(Ease.Linear);
        _fade.DOFade(1, _timeFade / 2f).SetLink(_fade.gameObject).SetEase(Ease.Linear).OnKill(() => {
            _menuPanel.SetActive(true);
            _gamePanel.SetActive(false);
        });
        LevelManager.Instance.ClearLevel();
    }
    public void PlayFXDown(float time)
    {
        _fade.transform.position = _upperPosition.position;
        _fade.transform.DOMove(_bottomPosition.position, time).SetLink(_fade.gameObject).SetEase(Ease.Linear);
        _fade.DOFade(1, time / 2f).SetLink(_fade.gameObject).SetEase(Ease.Linear).OnKill(() => {
            OnAnimOnMid?.Invoke();
        });
    }
    public void OpenSlot()
    {
        _slotPanel.SetActive(true);
        SlotUI.Instance.OnSlotOpened();
    }
    public void PlayNextLevel()
    {
        PlayerPrefs.SetInt("SlotOpened", 0);
        OpenGame();
    }
    public void PlayFXUp(float time)
    {
        _fade.transform.position = _bottomPosition.position;
        _fade.transform.DOMove(_upperPosition.position, time).SetLink(_fade.gameObject).SetEase(Ease.Linear);
        _fade.DOFade(1, time / 2f).SetLink(_fade.gameObject).SetEase(Ease.Linear).OnKill(() => {
            OnAnimOnMid?.Invoke();
        });
    }
}
