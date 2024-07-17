using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    [SerializeField] private ElementType _type;

    private Image _bgImage;
    private Image _fgImage;

    private Level _level;

    private bool _isItemOpened;

    private Vector3 _startScale;

    public void Initialize(Level level, float delay, ElementType type = ElementType.None)
    {
        _level = level;

        if (type != ElementType.None)
            _type = type;

        _startScale = transform.localScale;

        transform.localScale = Vector3.zero;
        transform.DOScale(_startScale, 0.2f).SetLink(gameObject).SetDelay(delay).SetEase(Ease.OutBack).OnKill(() => {
            AudioSystem.Instance.PlaySound(AudioSystem.Instance.CardSpawned, Random.Range(0.9f, 1.1f), 0.6f);
        });

        GetComponent<Button>().onClick.AddListener(Open);

        _bgImage = GetComponent<Image>();
        _fgImage = transform.GetChild(0).GetComponent<Image>();

        _fgImage.sprite = SpriteBase.Instance.SymbolsSprites[(int)_type];
    }
    public void DebugMe()
    {
        Debug.Log(_type + " | " + _fgImage.sprite);
    }
    public void Open()
    {
        if (_isItemOpened)
            return;
        transform.DOScaleX(0, 0.2f).SetLink(gameObject).OnKill(() => {
            _bgImage.sprite = SpriteBase.Instance.OpenedItem;
            _fgImage.gameObject.SetActive(true);
            transform.DOScaleX(1, 0.2f).SetLink(gameObject);
        });
        AudioSystem.Instance.PlaySound(AudioSystem.Instance.CardTurned, Random.Range(0.9f, 1.1f), 0.6f);
        _isItemOpened = true;
        _level.OnItemOpened(this);
    }
    public void OnItemCorrect()
    {
        transform.DOScale(new Vector2(_startScale.x*-1.2f, _startScale.y * 1.2f), 0.2f).SetLink(gameObject).SetEase(Ease.OutBack).SetDelay(0.5f);
        transform.DOScale(new Vector2(_startScale.x * -1, _startScale.y), 0.2f).SetLink(gameObject).SetDelay(0.8f);

        transform.DOMove(FindACoupleUI.Instance.CardsButton.position, 0.5f).SetLink(gameObject).SetEase(Ease.InBack).SetDelay(1f);
        transform.DOScale(Vector3.zero, 0.5f).SetLink(gameObject).SetDelay(1.4f);

        Destroy(gameObject, 2f);
    }
    public void Close(float delay)
    {
        transform.DOShakeRotation(0.6f, 40).SetLink(gameObject).SetDelay(delay*0.7f);
        transform.DOScaleX(0, 0.2f).SetLink(gameObject).SetDelay(delay).OnKill(() => {
            _bgImage.sprite = SpriteBase.Instance.ClosedItem;
            _fgImage.gameObject.SetActive(false);
            transform.DOScaleX(-1, 0.2f).SetLink(gameObject);
        });
        _isItemOpened = false;
    }
    public ElementType Type => _type;
}
