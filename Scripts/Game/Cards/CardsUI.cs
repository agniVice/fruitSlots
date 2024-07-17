using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardsUI : MonoBehaviour
{
    public static CardsUI Instance { get; private set; }

    [SerializeField] private List<Image> _cardImages;
    [SerializeField] private List<TextMeshProUGUI> _rewardTexts;
    [SerializeField] private List<TextMeshProUGUI> _progressTexts;
    [SerializeField] private List<GameObject> _blocks;

    [SerializeField] private CanvasGroup _panel;
    [SerializeField] private List<Transform> _cardsTransforms;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        _panel.blocksRaycasts = false;
    }
    private void Start()
    {
        Initialize();
    }
    private void Initialize()
    {
        for (int i = 0; i < CardsSystem.Instance.Cards.Length; i++)
        {
            _cardImages[i].sprite = SpriteBase.Instance.SymbolsSprites[(int)CardsSystem.Instance.CardTypes[i]];
            _rewardTexts[i].text = "+" + CardsSystem.Instance.Rewards[i];
        }
    }
    public void OpenCards()
    {
        GameTimer.Instance.StopTimer();
        UpdateCards();
        _panel.blocksRaycasts = true;
        _panel.alpha = 0f;
        _panel.DOFade(1, 0.3f).SetLink(_panel.gameObject);

        float delay = 0.2f;
        foreach (var item in _cardsTransforms)
        {
            Vector3 startScale = item.localScale;
            item.localScale = Vector3.zero;
            item.DOScale(startScale, Random.Range(0.15f, 0.3f)).SetEase(Ease.OutBack).SetDelay(delay);
            delay += 0.05f;
        }
    }
    public void CloseCards()
    {
        GameTimer.Instance.ContinueTimer();
        _panel.blocksRaycasts = false;
        _panel.DOFade(0, 0.3f).SetLink(_panel.gameObject);
    }
    public void UpdateCards()
    {
        foreach (var block in _blocks)
            block.SetActive(false);

        for (int i = 0; i < CardsSystem.Instance.Cards.Length; i++)
        {
            _progressTexts[i].text = CardsSystem.Instance.Cards[i] + "/" + CardsSystem.Instance.MaxCards[i];

            if (CardsSystem.Instance.Cards[i] >= CardsSystem.Instance.MaxCards[i])
                _blocks[i].SetActive(true);
        }
    }
}
