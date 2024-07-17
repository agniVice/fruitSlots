using UnityEngine;

public class SpriteBase : MonoBehaviour
{
    public static SpriteBase Instance { get; private set; }

    public Sprite[] SymbolsSprites;
    public Sprite OpenedItem;
    public Sprite ClosedItem;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

}
