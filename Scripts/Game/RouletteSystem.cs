using UnityEngine;

public class RouletteSystem : MonoBehaviour
{
    public static RouletteSystem Instance { get; private set; }

    [SerializeField] private float _winChance;
    [SerializeField] private int _roulettePrice;
    public int RoulettePrice { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
        RoulettePrice = _roulettePrice;
    }
    public bool IsSpinWin()
    {
        PlayerBalance.Instance.ChangeBalance(-RoulettePrice);
        if (Random.Range(0, 100) > (_winChance + (UpgradeSystem.Instance.Upgrades[0] * 2)))
            return false;
        else 
            return true;
    }
}
