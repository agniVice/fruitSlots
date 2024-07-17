using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SlotElement : MonoBehaviour
{
    private Image _elementImage;

    private SlotDrum _drum;
    private ElementType _elementType;

    private Vector3 _startScale;

    private void Awake()
    {
        _startScale = transform.localScale;
        _elementImage = GetComponent<Image>();
    }
    public void Initialize(SlotDrum drum) => _drum = drum;
    public void SetElementType(object sender, ElementType type)
    {
        if (sender.GetType() == typeof(SlotDrum))
            _elementType = type;

        _elementImage.sprite = SpriteBase.Instance.SymbolsSprites[(int)_elementType];
    }
    public ElementType GetElementType()
    {
        return _elementType;
    }
    public void PlayWinFX()
    {
        transform.DOScale(_startScale * 1.2f, 0.4f).SetLink(gameObject).SetEase(Ease.OutBack);
        transform.DOShakeRotation(0.5f, 5f).SetLink(gameObject).SetDelay(0.4f);
        transform.DOScale(_startScale, 0.4f).SetLink(gameObject).SetEase(Ease.OutBack).SetDelay(0.8f);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("BottomTrigger"))
        {
            _drum.RemoveElement(this);
            _drum.OnElementSpin(this);
            transform.localPosition = (_drum.GetLastElementPosition() + new Vector2(0, 328));
            _drum.AddElement(this); 
        }
    }
}