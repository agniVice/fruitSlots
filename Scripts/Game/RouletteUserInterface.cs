using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RouletteUserInterface : MonoBehaviour
{
    public static RouletteUserInterface Instance { get; private set; }

    [SerializeField] private Quaternion _rotationStart;
    [SerializeField] private List<Vector3> _rotationsWin;
    [SerializeField] private List<Vector3> _rotationsFail;

    [SerializeField] private Transform _rouletteObject;

    [SerializeField] private CanvasGroup _panelRoulette;
    [SerializeField] private List<Transform> _rouletteAnimTransforms;
    [SerializeField] private Button _buttonSpin;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        _rotationStart = _rouletteObject.rotation;
    }
    public void OnSpin()
    {
        if (PlayerBalance.Instance.Balance < RouletteSystem.Instance.RoulettePrice)
            return;

        AudioSystem.Instance.PlaySound(AudioSystem.Instance.RouletteSpin, 1f);
        Spin(RouletteSystem.Instance.IsSpinWin());

        _buttonSpin.interactable = false;
    }
    private void Spin(bool isWin)
    {
        if (isWin)
        { 
            _rouletteObject.DORotate(new Vector3(0, 0, 4680) + _rotationsWin[Random.Range(0, _rotationsWin.Count)], 6.5f, RotateMode.FastBeyond360)
                     .SetEase(Ease.OutCubic)
                     .SetLink(_rouletteObject.gameObject).OnKill(() => {
                         LevelManager.Instance.OnRouletteWin();
                         AudioSystem.Instance.PlaySound(AudioSystem.Instance.LevelSuccess, 1f);
                         FindACoupleUI.Instance.OnRouletteWin();
                         Instance.CloseRoulette();
                     });
        }
        else
        {
            _rouletteObject.DORotate(new Vector3(0, 0, 3600) + _rotationsFail[Random.Range(0, _rotationsFail.Count)], 6.5f, RotateMode.FastBeyond360)
                     .SetEase(Ease.OutCubic)
                     .SetLink(_rouletteObject.gameObject).OnKill(() => {
                         AudioSystem.Instance.PlaySound(AudioSystem.Instance.LevelFailed, 1f);
                         Instance.CloseRoulette();
                     });
        }
    }
    public void CloseRoulette()
    {
        _panelRoulette.blocksRaycasts = false;
        _panelRoulette.DOFade(0, 0.2f).SetLink(_panelRoulette.gameObject).OnKill(() => {
            _panelRoulette.gameObject.SetActive(false);
        });
    }
    public void OpenRoulette()
    {
        if (PlayerBalance.Instance.Balance < RouletteSystem.Instance.RoulettePrice)
            return;
        _buttonSpin.interactable = true;
        _rouletteObject.rotation = _rotationStart;
        _panelRoulette.gameObject.SetActive(true);
        _panelRoulette.blocksRaycasts = true;
        _panelRoulette.alpha = 0f;
        _panelRoulette.DOFade(1, 0.3f).SetLink(_panelRoulette.gameObject);

        float delay = 0.2f;
        foreach (var item in _rouletteAnimTransforms)
        {
            Vector3 startScale = item.localScale;
            item.localScale = Vector3.zero;
            item.DOScale(startScale, Random.Range(0.15f, 0.3f)).SetEase(Ease.OutBack).SetDelay(delay);
            delay += 0.05f;
        }
    }
}