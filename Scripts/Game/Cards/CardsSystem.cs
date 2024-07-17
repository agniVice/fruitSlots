using System.Collections.Generic;
using UnityEngine;

public class CardsSystem : MonoBehaviour
{
    public static CardsSystem Instance { get; private set; }

    public List<ElementType> CardTypes;
    public int[] Cards;
    public int[] MaxCards;
    public int[] Rewards;

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
        for (int i = 0; i < Cards.Length; i++)
            Cards[i] = PlayerPrefs.GetInt("Cards" + i, 0);

        UpgradeUserInterface.Instance.UpdateUpgrades();
    }
    public void OnCardsCollected(ElementType type, int count)
    {
        if (CardTypes.Contains(type))
        {
            if (Cards[(int)type] >= MaxCards[(int)type])
                return;
            Cards[(int)type] += count;

            if (Cards[(int)type] >= MaxCards[(int)type])
            {
                PlayerBalance.Instance.ChangeBalance(Rewards[(int)type]);
                AudioSystem.Instance.PlaySound(AudioSystem.Instance.LastScroll, 1f);
            }

            Save();
        }
    }
    private void Save()
    {
        for (int i = 0; i < Cards.Length; i++)
            PlayerPrefs.SetInt("Cards" + i, Cards[i]);
    }
}