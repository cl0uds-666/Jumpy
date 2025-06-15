using UnityEngine;
using System.Collections.Generic;

// This attribute forces this script's Awake() method to run before all others.
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

    public void AddCoins(int amount)
    {
        totalCoins += amount;
        SaveData();
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

    private void LoadData()
    {
        totalCoins = PlayerPrefs.GetInt(TotalCoinsKey, 0);
        string ownedItemsString = PlayerPrefs.GetString(OwnedItemsKey, "");
        if (!string.IsNullOrEmpty(ownedItemsString))
        {
            ownedItemNames = new List<string>(ownedItemsString.Split(','));
        }
        // --- ADDED DEBUG LOG ---
        Debug.Log("InventoryManager loaded. Total coins from memory: " + totalCoins);
    }
}
