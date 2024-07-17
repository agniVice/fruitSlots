using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUserInterface : MonoBehaviour
{ 
    public static UpgradeUserInterface Instance { get; private set; }

    [SerializeField] private List<TextMeshProUGUI> _textsPrices;
    [SerializeField] private List<Button> _buttonsBuy;

    [SerializeField] private CanvasGroup _panel;
    [SerializeField] private List<Transform> _upgradeAnimTransforms;

    [SerializeField] private CanvasGroup _panelError;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        _panelError.blocksRaycasts= false;
        _panel.blocksRaycasts = false;
    }
    public void UpdateUpgrades()
    {
        foreach (var item in _buttonsBuy)
            item.interactable = true;

        for (int i = 0; i < UpgradeSystem.Instance.Upgrades.Length; i++)
        {
            if (UpgradeSystem.Instance.Upgrades[i] >= UpgradeSystem.Instance.MaxUpgrades[i])
            { 
                _textsPrices[i].text = "MAX";
                _buttonsBuy[i].interactable = false;
            }
            else
                _textsPrices[i].text = UpgradeSystem.Instance.Prices[UpgradeSystem.Instance.Upgrades[i]].ToString();
        }
    }
    public void OpenUpgrade()
    {
        GameTimer.Instance.StopTimer();

        UpdateUpgrades();

        _panel.blocksRaycasts = true;
        _panel.alpha = 0f;
        _panel.DOFade(1, 0.3f).SetLink(_panel.gameObject);

        float delay = 0.2f;
        foreach (var item in _upgradeAnimTransforms)
        {
            Vector3 startScale = item.localScale;
            item.localScale = Vector3.zero;
            item.DOScale(startScale, Random.Range(0.15f, 0.3f)).SetEase(Ease.OutBack).SetDelay(delay);
            delay += 0.05f;
        }
    }
    public void CloseUpgrade()
    {
        GameTimer.Instance.ContinueTimer();

        _panel.DOFade(0, 0.3f).SetLink(_panel.gameObject);
        _panel.blocksRaycasts = false;

        CloseError();
    }
    public void OpenError()
    {
        _panelError.DOFade(1, 0.3f).SetLink(_panelError.gameObject);
        _panelError.blocksRaycasts = true;
        _panelError.alpha = 0f;

    }
    public void CloseError()
    {
        _panelError.blocksRaycasts = false;
        _panelError.DOFade(0, 0.3f).SetLink(_panelError.gameObject);
    }
    public void OnUpgradeButtonClicked(int upgrade) => UpgradeSystem.Instance.Upgrade(upgrade);
}