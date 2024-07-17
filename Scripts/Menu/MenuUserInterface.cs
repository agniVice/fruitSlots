using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuUserInterface : MonoBehaviour
{
    public static MenuUserInterface Instance { get; private set; }

    [Header("Menu")]
    [SerializeField] private List<Transform> _buttonsMenu;
    [SerializeField] private CanvasGroup _groupMenu;
    
    [Header("Settings")]
    [SerializeField] private List<Transform> _buttonsSettings;
    [SerializeField] private CanvasGroup _groupSettings;

    [Space]
    [Header("Other")]
    [SerializeField] private Sprite _enabledButtonSprite;
    [SerializeField] private Sprite _disabledButtonSprite;

    [Space]
    [SerializeField] private Image _buttonMusicImage;
    [SerializeField] private Image _buttonSoundImage;
    [SerializeField] private TextMeshProUGUI _textMusic;
    [SerializeField] private TextMeshProUGUI _textSound;

    public const string EnabledText = "ENABLED";
    public const string DisabledText = "DISABLED";

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }
    private void Start()
    {
        OpenMenu();
    }
    private void OpenMenu()
    {
        _groupSettings.gameObject.SetActive(false);
        _groupMenu.gameObject.SetActive(true);
        _groupMenu.alpha = 0f;
        _groupMenu.DOFade(1, 0.3f).SetLink(_groupMenu.gameObject);

        foreach (var item in _buttonsMenu)
        {
            item.localScale = Vector3.zero;
            item.DOScale(1, Random.Range(0.15f, 0.4f)).SetLink(item.gameObject).SetEase(Ease.OutBack);
        }
    }
    private void OpenSettings()
    {
        UpdateSettings();

        _groupMenu.gameObject.SetActive(false);
        _groupSettings.gameObject.SetActive(true);
        _groupSettings.alpha = 0f;
        _groupSettings.DOFade(1, 0.3f).SetLink(_groupSettings.gameObject);

        foreach (var item in _buttonsSettings)
        {
            item.localScale = Vector3.zero;
            item.DOScale(1, Random.Range(0.15f, 0.4f)).SetLink(item.gameObject).SetEase(Ease.OutBack);
        }
    }
    public void OnPlay(bool isSlot = false)
    {
        GlobalUserInterface.Instance.OpenGame(isSlot);
    }
    public void OnSettings()
    {
        OpenSettings();
    }
    public void OnExit()
    {
        Application.Quit();
    }
    public void OnMusic()
    {
        AudioSystem.Instance.ToggleMusic();
        UpdateSettings();
    }
    public void OnReturn()
    {
        OpenMenu();
    }
    public void OnSound()
    {
        AudioSystem.Instance.ToggleSound();
        UpdateSettings();
    }
    private void UpdateSettings()
    {

        _textMusic.text = DisabledText;
        _textSound.text = DisabledText;

        _buttonMusicImage.sprite = _disabledButtonSprite;
        _buttonSoundImage.sprite = _disabledButtonSprite;

        if (AudioSystem.Instance.MusicEnabled)
        {
            _buttonMusicImage.sprite = _enabledButtonSprite;
            _textMusic.text = EnabledText;
        }

        if (AudioSystem.Instance.SoundEnabled)
        {
            _buttonSoundImage.sprite = _enabledButtonSprite;
            _textSound.text = EnabledText;
        }
    }
}
