using UnityEngine;

// This enum will help us categorize our shop items.
public enum ItemType
{
    PlayerShape,
    TrailEffect,
    PlayerColor // Example for a future item type
}

// The [CreateAssetMenu] attribute lets us create instances of this object in the Project window.
[CreateAssetMenu(fileName = "NewShopItem", menuName = "Shop/Shop Item")]
public class ShopItem : ScriptableObject
{
    [Header("Item Information")]
    [Tooltip("The name of the item that will be displayed in the shop.")]
    public string itemName;

    [Tooltip("A short description of the item.")]
    [TextArea]
    public string itemDescription;

    [Tooltip("The icon that will be shown in the shop UI.")]
    public Sprite itemIcon;

    [Header("Item Properties")]
    [Tooltip("The price of the item in coins.")]
    public int itemPrice;

    [Tooltip("The category of this item.")]
    public ItemType itemType;

    [Header("Game Object")]
    [Tooltip("The actual prefab to use when this item is equipped (e.g., the Duck model, the Fire Trail particle effect).")]
    public GameObject itemPrefab;
}
