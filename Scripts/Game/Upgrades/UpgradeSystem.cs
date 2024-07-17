using UnityEngine;

public class UpgradeSystem : MonoBehaviour
{
    public static UpgradeSystem Instance { get; private set; }

    public int[] Prices;
    public int[] Upgrades;
    public int[] MaxUpgrades;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }
    private void Start()
    {
        Initialize();
    }
    private void Initialize()
    {
        for (int i = 0; i < Upgrades.Length; i++)
            Upgrades[i] = PlayerPrefs.GetInt("Upgrade" + i, 0);

        UpgradeUserInterface.Instance.UpdateUpgrades();
    }
    private void Save()
    {
        for (int i = 0; i < Upgrades.Length; i++)
            PlayerPrefs.SetInt("Upgrade" + i, Upgrades[i]);
    }
    public void Upgrade(int upgrade)
    {
        if (Upgrades[upgrade] >= MaxUpgrades[upgrade])
            return;
        if (PlayerBalance.Instance.Balance < Prices[Upgrades[upgrade]])
        {
            UpgradeUserInterface.Instance.OpenError();
            return;
        }

        AudioSystem.Instance.PlaySound(AudioSystem.Instance.UpgradeSound, 1f);

        PlayerBalance.Instance.ChangeBalance(-Prices[Upgrades[upgrade]]);

        SlotUI.Instance.UpdateBalance();

        Upgrades[upgrade]++;

        UpgradeUserInterface.Instance.UpdateUpgrades();

        Save();
    }
}