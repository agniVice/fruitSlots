using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FindACoupleUI : MonoBehaviour
{
    public static FindACoupleUI Instance { get; private set; }

    [Header("Game")]
    public Transform CardsButton;

    [SerializeField] private Image _timerAlertImage;
    [SerializeField] private Transform _alert;
    [SerializeField] private Transform _timer;

    [SerializeField] private TextMeshProUGUI _couplesText;
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private TextMeshProUGUI _timerAlertText;
    [SerializeField] private TextMeshProUGUI _healthText;
    [SerializeField] private TextMeshProUGUI _levelNumberText;
    [SerializeField] private TextMeshProUGUI _rewardCountText;
    [SerializeField] private TextMeshProUGUI _balanceText;
    [SerializeField] private TextMeshProUGUI _spinsText;

    [SerializeField] private Image _incorrectFX;

    [SerializeField] private Image _health;
    [SerializeField] private Color32 _correctColor = new Color32(108, 255, 108, 255);
    [SerializeField] private Color32 _incorrectColor = new Color32(255, 108, 108, 255);

    [Space]
    [Header("LevelFailed")]
    [SerializeField] private CanvasGroup _failWindow;
    [SerializeField] private List<Transform> _failTransforms;
    [SerializeField] private TextMeshProUGUI _tryingCostText;

    [Space]
    [Header("LevelSuccess")]
    [SerializeField] private CanvasGroup _successWindow;
    [SerializeField] private List<Transform> _successTransforms;
    [SerializeField] private TextMeshProUGUI _successRewardCountText;
    [SerializeField] private TextMeshProUGUI _successRewardSpinsCountText;

    [Space]
    [Header("Roulette")]
    [SerializeField] private TextMeshProUGUI _roulettePriceText;
    [SerializeField] private Button _rouletteBuy;

    [Space]
    [Header("Tutorial")]
    [SerializeField] private CanvasGroup _tutorialPanel;
    [SerializeField] private List<Transform> _tutorialTransfroms;

    [Space]
    [Header("Tip")]
    [SerializeField] private CanvasGroup _tipPanel;
    [SerializeField] private List<Transform> _tipTransforms;
    [SerializeField] private Button _takeTip;
    [SerializeField] private TextMeshProUGUI _tipCost;

    private Vector3 _healthScale;
    private int _lastBalance = 0;
    private int _currentBalance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        _healthScale = _health.transform.localScale;
    }
    private void Start()
    {
        PlayerBalance.Instance.OnBalanceChanged += UpdateBalance;
    }
    private void OnDisable()
    {
        PlayerBalance.Instance.OnBalanceChanged -= UpdateBalance;
    }
    private void FixedUpdate()
    {
        _timerText.text = Mathf.Round(GameTimer.Instance.Timer) + " SEC";
    }
    public void OpenTutorial()
    {
        GameTimer.Instance.StopTimer();
        _tutorialPanel.blocksRaycasts = true;
        _tutorialPanel.alpha = 0f;
        _tutorialPanel.DOFade(1, 0.3f).SetLink(_tutorialPanel.gameObject);

        float delay = 0.3f;
        foreach (var item in _tutorialTransfroms)
        {
            Vector3 startScale = item.localScale;
            item.localScale = Vector3.zero;
            item.DOScale(startScale, Random.Range(0.15f, 0.3f)).SetLink(item.gameObject).SetEase(Ease.OutBack).SetDelay(delay);
            delay += 0.1f;
        }
    }
    public void CloseTutorial()
    {
        GameTimer.Instance.ContinueTimer();
        _tutorialPanel.blocksRaycasts = false;
        _tutorialPanel.DOFade(0, 0.4f).SetLink(_tutorialPanel.gameObject);
        PlayerPrefs.SetInt("Tutorial", 1);
        LevelManager.Instance.SpawnLevel();
    }
    public void OnGameOver()
    {
        int roulettePrice = RouletteSystem.Instance.RoulettePrice;
        _roulettePriceText.text = roulettePrice.ToString();
        if (PlayerBalance.Instance.Balance >= roulettePrice)
            _rouletteBuy.interactable = true;
        else
            _rouletteBuy.interactable = false;

        _failWindow.alpha = 0f;
        _failWindow.gameObject.SetActive(true);
        _failWindow.DOFade(1f, 0.5f).SetLink(_failWindow.gameObject);
        float delay = 0.3f;
        foreach (var item in _failTransforms)
        {
            Vector3 startScale = item.localScale;
            item.localScale = Vector3.zero;
            item.DOScale(startScale, Random.Range(0.15f, 0.3f)).SetLink(item.gameObject).SetEase(Ease.OutBack).SetDelay(delay);
            delay += 0.1f;
        }
    }
    public void OnGameSuccess()
    {
        _successWindow.alpha = 0f;
        _successWindow.gameObject.SetActive(true);
        _successWindow.DOFade(1f, 0.5f).SetLink(_successWindow.gameObject);
        float delay = 0.3f;
        foreach (var item in _successTransforms)
        {
            Vector3 startScale = item.localScale;
            item.localScale = Vector3.zero;
            item.DOScale(startScale, Random.Range(0.15f, 0.3f)).SetLink(item.gameObject).SetEase(Ease.OutBack).SetDelay(delay);
            delay += 0.1f;
        }

        _successRewardCountText.text = _rewardCountText.text;
        _successRewardSpinsCountText.text = LevelManager.Instance.GetSpinsReward().ToString();
    }
    public void TimerAlert()
    {
        _timerAlertImage.gameObject.SetActive(true);
        _timerAlertImage.color = new Color32(0, 0, 0, 200);

        _alert.localScale = Vector3.one;
        _alert.localPosition = Vector3.zero;

        Vector3 timerScale = _timer.localScale;

        _timerAlertImage.DOFade(0, 0.4f).SetLink(_timerAlertImage.gameObject).SetDelay(1.5f);
        _alert.DOScale(0, 0.4f).SetLink(_alert.gameObject).SetDelay(1.5f).OnKill(() => { _timerAlertImage.gameObject.SetActive(false); });
        _timer.DOScale(timerScale * 1.2f, 0.2f).SetLink(_timer.gameObject).SetDelay(1.5f);
        _timer.DOScale(timerScale, 0.2f).SetLink(_timer.gameObject).SetDelay(1.8f).SetEase(Ease.InBack);
    }
    public void UpdateHealth() => _healthText.text = PlayerHealth.Instance.Health.ToString();
    public void DecreaseHealth()
    {
        _incorrectFX.color = _incorrectColor;
        _health.transform.DOScale(_healthScale * 1.25f, 0.3f).SetLink(_health.gameObject);
        _health.transform.DOScale(_healthScale, 0.3f).SetLink(_health.gameObject).SetDelay(0.3f).SetEase(Ease.InBack);
        _health.transform.DOShakeRotation(0.3f, 20f).SetLink(_health.gameObject).SetDelay(0.2f);
        _incorrectFX.DOFade(1, 1f).SetLink(_incorrectFX.gameObject);
        _incorrectFX.DOFade(0, 0.5f).SetLink(_incorrectFX.gameObject).SetDelay(0.5f);
        UpdateHealth();
    }
    public void OnCorrectCouple()
    {
        _incorrectFX.color = _correctColor;
        _incorrectFX.DOFade(1, 1f).SetLink(_incorrectFX.gameObject);
        _incorrectFX.DOFade(0, 0.5f).SetLink(_incorrectFX.gameObject).SetDelay(0.5f);
    }
    public void UpdateBalance()
    {
        _spinsText.text = PlayerBalance.Instance.Spins.ToString();
        _currentBalance = _lastBalance;
        DOTween.To(() => _currentBalance, x => _currentBalance = x, PlayerBalance.Instance.Balance, 1f).SetEase(Ease.Linear).OnUpdate(UpdateBalanceText);
    }
    public void UpdateBalanceText()
    {
        _balanceText.text = _currentBalance.ToString();
        _lastBalance = PlayerBalance.Instance.Balance;
    }
    public void OnRouletteWin()
    {
        _failWindow.DOFade(0, 1f).SetLink(_failWindow.gameObject).OnKill(() => { _failWindow.gameObject.SetActive(false); });
    }
    public void OnRestartButtonClicked()
    {
        LevelManager.Instance.RestartLevel();
        //GlobalUI.Instance.OnAnimOnMid += LevelManager.Instance.RestartLevel;
        GlobalUserInterface.Instance.PlayFXDown(1);
        transform.DOScale(transform.localScale, 1.2f).SetLink(gameObject);
        _successWindow.DOFade(0, 0.5f).SetLink(_failWindow.gameObject).OnKill(() => { _successWindow.gameObject.SetActive(false); });
        _failWindow.DOFade(0, 1f).SetLink(_failWindow.gameObject).OnKill(() => { _failWindow.gameObject.SetActive(false); });
    }
    public void OnPlayNextButtonClicked()
    {
        GlobalUserInterface.Instance.OpenGame();
        _successWindow.DOFade(0, 0.5f).SetLink(_failWindow.gameObject).OnKill(() => { _successWindow.gameObject.SetActive(false); });
        _failWindow.DOFade(0, 1f).SetLink(_failWindow.gameObject).OnKill(() => { _failWindow.gameObject.SetActive(false); });
    }
    public void OnSlotButtonClicked()
    {
        LevelManager.Instance.ClearLevel();
        GlobalUserInterface.Instance.OpenGame(true);
        _successWindow.DOFade(0, 0.5f).SetLink(_failWindow.gameObject).OnKill(() => { _successWindow.gameObject.SetActive(false); });
        _failWindow.DOFade(0, 1f).SetLink(_failWindow.gameObject).OnKill(() => { _failWindow.gameObject.SetActive(false); });
    }
    public void OnMenuButtonClicked()
    {
        _successWindow.DOFade(0, 1f).SetLink(_failWindow.gameObject).OnKill(() => { _successWindow.gameObject.SetActive(false); });
        _failWindow.DOFade(0, 1f).SetLink(_failWindow.gameObject).OnKill(() => { _failWindow.gameObject.SetActive(false); });
        GlobalUserInterface.Instance.OpenMenu();
    }
    public void OnTipButtonClicked()
    {
        _tipCost.text = LevelManager.Instance.TipCost.ToString();
        GameTimer.Instance.StopTimer();
        _tipPanel.alpha = 0f;
        _tipPanel.blocksRaycasts = true;
        _tipPanel.DOFade(1f, 0.5f).SetLink(_tipPanel.gameObject);
        float delay = 0.3f;
        foreach (var item in _tipTransforms)
        {
            Vector3 startScale = item.localScale;
            item.localScale = Vector3.zero;
            item.DOScale(startScale, Random.Range(0.15f, 0.3f)).SetLink(item.gameObject).SetEase(Ease.OutBack).SetDelay(delay);
            delay += 0.1f;
        }
        _takeTip.interactable = (PlayerBalance.Instance.Balance >= LevelManager.Instance.TipCost);
    }
    public void OnTakeTipButtonClicked()
    {
        if (PlayerBalance.Instance.Balance >= LevelManager.Instance.TipCost)
        {
            _takeTip.interactable = true;
            LevelManager.Instance.TakeTip();
            CloseTip();
        }
        else
            _takeTip.interactable = false;
    }
    public void CloseTip()
    {
        GameTimer.Instance.ContinueTimer();
        _tipPanel.blocksRaycasts = false;
        _tipPanel.DOFade(0, 0.4f).SetLink(_tipPanel.gameObject);
    }
    public void UpdateLevelNumber(int number) => _levelNumberText.text = "LEVEL " + number.ToString();
    public void UpdateRewardCount(int count) => _rewardCountText.text = "+" + count.ToString();
    public void UpdateTimerAlert() => _timerAlertText.text = Mathf.Round(GameTimer.Instance.Timer) + " SEC";
    public void UpdateCouples(int found, int all) => _couplesText.text = found + "/" + all;
    public void OnCardsChanged(float delay) => CardsButton.DOShakeRotation(0.5f, 20f).SetLink(CardsButton.gameObject).SetDelay(delay).OnKill(() => { CardsButton.rotation = Quaternion.identity; });
}
