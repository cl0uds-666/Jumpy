using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ShopUIController : MonoBehaviour
{
    [Header("Shop UI References")]
    [SerializeField] private Transform itemContainer;
    [SerializeField] private GameObject shopItemUIPrefab;
    [SerializeField] private TextMeshProUGUI coinText;

    private List<ShopItem> shopItems = new List<ShopItem>();

    // Start is called after all Awake() methods have been completed.
    void Start()
    {
        // --- ADDED DEBUG LOG ---
        if (coinText == null) { Debug.LogError("Shop Error: Coin Text has not been assigned in the Inspector!"); }
        if (InventoryManager.Instance == null) { Debug.LogError("Shop Error: Could not find InventoryManager Instance!"); }

        LoadItems();
        RefreshShop(); // Call RefreshShop once everything is ready.
    }

    // OnEnable is good for refreshing the view if the player navigates away and comes back.
    void OnEnable()
    {
        // We only refresh here if the shop has already been built once.
        if (shopItems.Count > 0)
        {
            RefreshShop();
        }
    }

    private void LoadItems()
    {
        shopItems.Clear();
        var items = Resources.LoadAll<ShopItem>("");
        shopItems.AddRange(items);
    }

    private void BuildShop()
    {
        foreach (Transform child in itemContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (ShopItem item in shopItems)
        {
            GameObject itemObject = Instantiate(shopItemUIPrefab, itemContainer);
            ShopItemUI uiScript = itemObject.GetComponent<ShopItemUI>();
            uiScript.Setup(item, this);
        }
    }

    public void RefreshShop()
    {
        if (coinText != null && InventoryManager.Instance != null)
        {
            int currentCoins = InventoryManager.Instance.GetTotalCoins();
            // --- ADDED DEBUG LOG ---
            Debug.Log("ShopUIController is refreshing. Attempting to set coin text with value: " + currentCoins);
            coinText.text = "Coins: " + currentCoins;
        }

        BuildShop();
    }
}
