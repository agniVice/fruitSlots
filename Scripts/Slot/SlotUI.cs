using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotUI : MonoBehaviour
{
    public static SlotUI Instance { get; private set; }

    public bool CanSpin { get; private set; } = true;

    [SerializeField] private TextMeshProUGUI _balanceText;
    [SerializeField] private TextMeshProUGUI _spinsText;
    [SerializeField] private TextMeshProUGUI _betText;
    [SerializeField] private TextMeshProUGUI _winText;
    [SerializeField] private TextMeshProUGUI _bigWinText;

    [SerializeField] private Image _spinImage;
    [SerializeField] private Image _autoPlayImage;

    [SerializeField] private Button _increaseBetButton;
    [SerializeField] private Button _decreaseBetButton;
    [SerializeField] private Button _nextLevelButton;

    [SerializeField] private Sprite _activeIncreaseButton;
    [SerializeField] private Sprite _inactiveIncreaseButton;

    [SerializeField] private Sprite _activeDecreaseButton;
    [SerializeField] private Sprite _inactiveDecreaseButton;

    [SerializeField] private Sprite _activeAutoPlayButton;
    [SerializeField] private Sprite _inactiveAutoPlayButton;

    [SerializeField] private Sprite _activeSpin;
    [SerializeField] private Sprite _inactiveSpin;

    [SerializeField] private CanvasGroup _winPanel;
    [SerializeField] private CanvasGroup _bigWinPanel;

    [SerializeField] private Image _bigWinImage;
    [SerializeField] private Image[] _winLines;

    [SerializeField] private float _spinButtonTime;


    private int _lastBalance = 0;
    private int _currentBalance;
    private int _currentWin;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }
    public void OnSlotOpened()
    {
        UpdateBalance();
        UpdateBet();
    }
    public void UpdateBet()
    {
        _increaseBetButton.interactable = true;
        _decreaseBetButton.interactable = true;

        if (SlotMachine.Instance.IsCurrentBetMax())
            _increaseBetButton.interactable = false;
        else
            _increaseBetButton.interactable = true;

        if (SlotMachine.Instance.IsCurrentBetMin())
            _decreaseBetButton.interactable = false;
        else
            _decreaseBetButton.interactable = true;

        _betText.text = SlotMachine.Instance.FinalBet.ToString();
    }
    public void OnPlayerWin(int value, int[] lines, int wildCount)
    {
        int win = 0;
        foreach (int line in lines) 
        {
            if(line != 0)
                win++;
        }
        if (win == 0)
            return;
        CanSpin = false;
        for (int l = 0; l < lines.Length; l++)
        {
            _winLines[l].color = new Color32(255, 255, 255, 0);
            if (lines[l] == 1)
            {
                _winLines[l].DOFade(1, 0.2f).SetLink(_winLines[l].gameObject);
                _winLines[l].DOFade(0, 0.2f).SetLink(_winLines[l].gameObject).SetDelay(0.2f);

                _winLines[l].DOFade(1, 0.2f).SetLink(_winLines[l].gameObject).SetDelay(0.4f);
                _winLines[l].DOFade(0, 0.2f).SetLink(_winLines[l].gameObject).SetDelay(0.6f);

                _winLines[l].DOFade(1, 0.2f).SetLink(_winLines[l].gameObject).SetDelay(0.8f);
            }
        }
        if (wildCount != 0)
        {
            OnPlayerBigWin(value);
            return;
        }
        AudioSystem.Instance.PlaySound(AudioSystem.Instance.DefaultWin, 1f);
        _currentWin = 0;
        _winPanel.alpha = 0;
        _winPanel.DOFade(1, 0.4f).SetLink(_winPanel.gameObject);
        _winPanel.DOFade(0, 0.4f).SetLink(_winPanel.gameObject).SetDelay(1.1f);

        transform.DOScale(transform.localScale, 1.1f).SetLink(gameObject).OnKill(() => { CanSpin = true; });

        DOTween.To(() => _currentWin, x => _currentWin = x, value, 0.4f).OnUpdate(UpdateWinText).OnKill(() => {UpdateBalance(0.5f);});
    }
    public void OnPlayerBigWin(int value)
    {
        AudioSystem.Instance.PlaySound(AudioSystem.Instance.BigWin, 1f);

        _bigWinPanel.blocksRaycasts = true;

        _currentWin = 0;
        _bigWinPanel.alpha = 0;
        _bigWinPanel.DOFade(1, 0.4f).SetLink(_bigWinPanel.gameObject);
        _bigWinPanel.DOFade(0, 0.4f).SetLink(_bigWinPanel.gameObject).SetDelay(2f);


        Vector3 startScale = _bigWinImage.transform.localScale;
        _bigWinImage.color = new Color32(255, 255, 255, 0);

        _bigWinImage.DOFade(1, 0.4f).SetLink(_bigWinImage.gameObject);
        _bigWinImage.transform.DOScale(startScale, 0.4f).SetLink(_bigWinImage.gameObject).SetEase(Ease.OutBack);
        _bigWinImage.transform.DOShakeRotation(1f, 5f).SetLink(_bigWinImage.gameObject);

        transform.DOScale(transform.localScale, 3f).SetLink(gameObject).OnKill(() => { CanSpin = true; _bigWinPanel.blocksRaycasts = false; });

        DOTween.To(() => _currentWin, x => _currentWin = x, value, 2f).OnUpdate(UpdateBigWinText).OnKill(() => { UpdateBalance(0.5f); });
    }
    public void UpdateBalance()
    {
        _spinsText.text = PlayerBalance.Instance.Spins.ToString();
        _currentBalance = _lastBalance;
        DOTween.To(() => _currentBalance, x => _currentBalance = x, PlayerBalance.Instance.Balance, 1f).SetEase(Ease.Linear).OnUpdate(UpdateBalanceText);
    }
    public void UpdateBalance(float delay = 0f)
    {
        _spinsText.text = PlayerBalance.Instance.Spins.ToString();
        _currentBalance = _lastBalance;
        DOTween.To(() => _currentBalance, x => _currentBalance = x, PlayerBalance.Instance.Balance, 1f).SetEase(Ease.Linear).OnUpdate(UpdateBalanceText).SetDelay(delay);
    }
    private void UpdateBalanceText()
    {
        _balanceText.text = _currentBalance.ToString();
        _lastBalance = PlayerBalance.Instance.Balance;
    }
    public void UpdateAutoSpin()
    {
        if (SlotMachine.Instance.AutoSpinnig)
            _autoPlayImage.sprite = _inactiveAutoPlayButton;
        else
            _autoPlayImage.sprite = _activeAutoPlayButton;
    }
    public void OnIncreaseBetButtonClicked()
    {
        SlotMachine.Instance.IncreaseBet();
        UpdateBet();
    }
    public void OnDecreaseBetButtonClicked()
    {
        SlotMachine.Instance.DecreaseBet();
        UpdateBet();
    }
    public void OnStartSpin()
    {
        _nextLevelButton.interactable = false;
        _spinImage.sprite = _inactiveSpin;
        _spinImage.transform.DORotate(new Vector3(0, 0, 540f), _spinButtonTime, RotateMode.FastBeyond360).SetEase(Ease.InOutBack);
    }
    public void OnEndSpin()
    {
        _nextLevelButton.interactable = true;
        _spinImage.sprite = _activeSpin;
    }
    public void OnAutoSpinButtonClicked()
    {
        if (!SlotMachine.Instance.AutoSpinnig && (PlayerBalance.Instance.Balance == 0 || PlayerBalance.Instance.Spins ==0))
            return;
        SlotMachine.Instance.ToggleAutoSpin();
        UpdateAutoSpin();
    }
    public void OnSpinButtonClicked() => SlotMachine.Instance.Spin();
    private void UpdateWinText() => _winText.text = "+" + _currentWin.ToString();
    private void UpdateBigWinText() => _bigWinText.text = "+" + _currentWin.ToString();
}
