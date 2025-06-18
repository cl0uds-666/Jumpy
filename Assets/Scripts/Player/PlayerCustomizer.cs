using UnityEngine;
using System.Collections.Generic;

public class PlayerCustomizer : MonoBehaviour
{
    [Header("Customization Points")]
    [SerializeField] private Transform shapeContainer;
    [SerializeField] private GameObject defaultVisuals;

    [Header("Item Database")]
    [SerializeField] private List<ShopItem> allShopItems = new List<ShopItem>();
    [SerializeField] private ShopItem defaultSoundsItem;

    private PlayerSoundManager playerSoundManager;
    private Dictionary<string, ShopItem> itemDatabase = new Dictionary<string, ShopItem>();

    void Awake()
    {
        playerSoundManager = GetComponent<PlayerSoundManager>();
        BuildDatabase();
        ApplyCustomizations();
    }

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

    private void ApplyCustomizations()
    {
        string equippedShapeName = InventoryManager.Instance.GetEquippedShapeName();
        ShopItem equippedItem = null;

        foreach (Transform child in shapeContainer) { Destroy(child.gameObject); }

        if (equippedShapeName == "DefaultShape" || !itemDatabase.TryGetValue(equippedShapeName, out equippedItem))
        {
            if (defaultVisuals != null) { defaultVisuals.SetActive(true); }
            equippedItem = defaultSoundsItem;
        }
        else
        {
            if (defaultVisuals != null) { defaultVisuals.SetActive(false); }
            Instantiate(equippedItem.itemPrefab, shapeContainer);
        }

        if (playerSoundManager != null && equippedItem != null)
        {
            // The Initialize function now handles all sounds automatically.
            playerSoundManager.Initialize(equippedItem, defaultSoundsItem);
        }
    }
}
