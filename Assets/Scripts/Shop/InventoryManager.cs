using UnityEngine;
using System.Collections.Generic;

[DefaultExecutionOrder(-100)]
public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    private int totalCoins;
    private List<string> ownedItemNames = new List<string>();

    private const string TotalCoinsKey = "PlayerTotalCoins";
    private const string OwnedItemsKey = "PlayerOwnedItems";
    private const string EquippedShapeKey = "PlayerEquippedShape";
    private const string EquippedTrailKey = "PlayerEquippedTrail";

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); }
        else { Instance = this; DontDestroyOnLoad(gameObject); LoadData(); }
    }

    private void LoadData()
    {
        totalCoins = PlayerPrefs.GetInt(TotalCoinsKey, 0);

        string ownedItemsString = PlayerPrefs.GetString(OwnedItemsKey, "");
        if (!string.IsNullOrEmpty(ownedItemsString))
        {
            ownedItemNames = new List<string>(ownedItemsString.Split(','));
        }

        // --- NEW LOGIC: Grant Default Items ---
        // After loading, we check if the player owns the default shape. If not, we grant it for free.
        // This ensures that even new players "own" the default cube.
        if (!ownedItemNames.Contains("DefaultShape"))
        {
            ownedItemNames.Add("DefaultShape");
        }
        // You could add another check here for a "DefaultTrail" if you add one later.
    }

    public bool PurchaseItem(ShopItem item)
    {
        if (totalCoins >= item.itemPrice && !IsItemOwned(item))
        {
            totalCoins -= item.itemPrice;
            ownedItemNames.Add(item.name);
            SaveData();
            return true;
        }
        return false;
    }

    // ... (The rest of the script is unchanged) ...
    #region Unchanged Methods
    public void AddCoins(int amount)
    {
        totalCoins += amount;
        SaveData();
    }

    public void EquipItem(ShopItem item)
    {
        if (!IsItemOwned(item)) return;
        if (item.itemType == ItemType.PlayerShape) { PlayerPrefs.SetString(EquippedShapeKey, item.name); }
        else if (item.itemType == ItemType.TrailEffect) { PlayerPrefs.SetString(EquippedTrailKey, item.name); }
        PlayerPrefs.Save();
    }

    public bool IsItemOwned(ShopItem item) => ownedItemNames.Contains(item.name);
    public string GetEquippedShapeName() => PlayerPrefs.GetString(EquippedShapeKey, "DefaultShape");
    public string GetEquippedTrailName() => PlayerPrefs.GetString(EquippedTrailKey, "DefaultTrail");
    public int GetTotalCoins() => totalCoins;

    private void SaveData()
    {
        PlayerPrefs.SetInt(TotalCoinsKey, totalCoins);
        string ownedItemsString = string.Join(",", ownedItemNames);
        PlayerPrefs.SetString(OwnedItemsKey, ownedItemsString);
        PlayerPrefs.Save();
    }
    #endregion
}
