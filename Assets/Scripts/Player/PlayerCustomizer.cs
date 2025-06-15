using UnityEngine;
using System.Collections.Generic; // For using Dictionaries

// This script customizes the player's appearance at the start of the game
// based on what items are equipped in the InventoryManager.
public class PlayerCustomizer : MonoBehaviour
{
    [Header("Customization Points")]
    [Tooltip("The parent object where the new player shape prefab will be spawned.")]
    [SerializeField] private Transform shapeContainer;
    [Tooltip("A reference to the default cube visual so it can be turned off.")]
    [SerializeField] private GameObject defaultVisuals;

    [Header("Item Database")]
    [Tooltip("A list of all possible ShopItem data assets in the game.")]
    [SerializeField] private List<ShopItem> allShopItems = new List<ShopItem>();

    // A dictionary for fast lookups. We find items by their name.
    private Dictionary<string, ShopItem> itemDatabase = new Dictionary<string, ShopItem>();

    void Awake()
    {
        // At the very start, build the database for quick access.
        BuildDatabase();

        // Customize the player.
        ApplyCustomizations();
    }

    /// <summary>
    /// Loads all ShopItem assets into a fast-lookup dictionary.
    /// </summary>
    private void BuildDatabase()
    {
        foreach (ShopItem item in allShopItems)
        {
            if (item != null && !itemDatabase.ContainsKey(item.name))
            {
                itemDatabase.Add(item.name, item);
            }
        }
    }

    /// <summary>
    /// Reads the equipped item names from the InventoryManager and applies the visuals.
    /// </summary>
    private void ApplyCustomizations()
    {
        // --- Apply Player Shape ---
        string equippedShapeName = InventoryManager.Instance.GetEquippedShapeName();
        if (itemDatabase.TryGetValue(equippedShapeName, out ShopItem shapeItem))
        {
            // If we found a valid equipped item...
            // Deactivate the default cube visuals.
            if (defaultVisuals != null)
            {
                defaultVisuals.SetActive(false);
            }
            // Instantiate the new shape's prefab inside our container.
            Instantiate(shapeItem.itemPrefab, shapeContainer);
        }
        else
        {
            // If no custom shape is equipped, ensure the default visuals are on.
            if (defaultVisuals != null)
            {
                defaultVisuals.SetActive(true);
            }
        }

        // --- Apply Trail Effect (You can add this logic later) ---
        // string equippedTrailName = InventoryManager.Instance.GetEquippedTrailName();
        // if (itemDatabase.TryGetValue(equippedTrailName, out ShopItem trailItem))
        // {
        //     Instantiate(trailItem.itemPrefab, transform); // Attach trail directly to the player
        // }
    }
}
