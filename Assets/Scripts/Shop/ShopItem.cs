using UnityEngine;

public enum ItemType
{
    PlayerShape,
    TrailEffect
}

[CreateAssetMenu(fileName = "NewShopItem", menuName = "Shop/Shop Item")]
public class ShopItem : ScriptableObject
{
    [Header("Item Information")]
    public string itemName;
    [TextArea]
    public string itemDescription;
    public Sprite itemIcon;

    [Header("Item Properties")]
    public int itemPrice;
    public ItemType itemType;

    [Header("Game Object")]
    public GameObject itemPrefab;

    [Header("Custom Sound Effects (Optional)")]
    public AudioClip jumpSound;
    public AudioClip landingSound;
    public AudioClip explosionSound; // NEW
}
