using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image itemIconImage;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemPriceText;
    [SerializeField] private Button purchaseButton;

    private ShopItem assignedItem;
    private ShopUIController shopController; // A reference to the main controller.

    public void Setup(ShopItem itemToSetup, ShopUIController controller)
    {
        assignedItem = itemToSetup;
        shopController = controller; // Store the reference.

        itemIconImage.sprite = assignedItem.itemIcon;
        itemNameText.text = assignedItem.itemName;

        if (InventoryManager.Instance.IsItemOwned(assignedItem))
        {
            itemPriceText.text = "Owned";
            purchaseButton.GetComponentInChildren<TextMeshProUGUI>().text = "Equip";
            if (IsEquipped())
            {
                purchaseButton.interactable = false;
                purchaseButton.GetComponentInChildren<TextMeshProUGUI>().text = "Equipped";
            }
        }
        else
        {
            itemPriceText.text = assignedItem.itemPrice.ToString();
            purchaseButton.GetComponentInChildren<TextMeshProUGUI>().text = "Buy";
        }

        purchaseButton.onClick.RemoveAllListeners(); // Clear old listeners first.
        purchaseButton.onClick.AddListener(OnItemButtonClicked);
    }

    private void OnItemButtonClicked()
    {
        if (InventoryManager.Instance.IsItemOwned(assignedItem))
        {
            InventoryManager.Instance.EquipItem(assignedItem);
            shopController.RefreshShop(); // Refresh the shop to show the change.
        }
        else
        {
            bool success = InventoryManager.Instance.PurchaseItem(assignedItem);
            if (success)
            {
                shopController.RefreshShop(); // Refresh the shop after a successful purchase.
            }
        }
    }

    private bool IsEquipped()
    {
        if (assignedItem.itemType == ItemType.PlayerShape)
        {
            return InventoryManager.Instance.GetEquippedShapeName() == assignedItem.name;
        }
        else if (assignedItem.itemType == ItemType.TrailEffect)
        {
            return InventoryManager.Instance.GetEquippedTrailName() == assignedItem.name;
        }
        return false;
    }
}
