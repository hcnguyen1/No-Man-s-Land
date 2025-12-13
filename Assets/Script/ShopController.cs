using UnityEngine;
using Inventory.UI;
using Inventory.Model;
using Inventory;

public class ShopController : MonoBehaviour
{
    [SerializeField]
    private ShopUI shopUI;

    [SerializeField]
    private ShopInventorySO shopInventory;

    [SerializeField]
    private InventorySO playerInventory;

    [SerializeField]
    private InventoryController inventoryController; // Reference to inventory controller

    [SerializeField]
    private Player player;

    [SerializeField]
    private AudioClip purchaseSuccessClip;

    [SerializeField]
    private AudioClip purchaseFailClip;

    [SerializeField]
    private AudioSource audioSource;

    private bool isShopOpen = false;

    private void Start()
    {
        if (player == null)
        {
            player = FindObjectOfType<Player>();
        }

        if (inventoryController == null)
        {
            inventoryController = FindObjectOfType<InventoryController>();
        }

        if (shopUI != null)
        {
            shopUI.OnItemPurchaseRequested += HandlePurchaseRequest;
        }
    }

    public void OpenShop()
    {
        if (shopUI != null && shopInventory != null && player != null)
        {
            isShopOpen = true;
            
            // Initialize shop
            shopUI.InitializeShop(shopInventory.GetShopItems(), player.currency);
            shopUI.Show();

            // Open inventory
            if (inventoryController != null)
            {
                inventoryController.ShowInventory();
            }
        }
    }

    public void CloseShop()
    {
        if (shopUI != null)
        {
            isShopOpen = false;
            shopUI.Hide();

            // Close player inventory when shop closes
            if (inventoryController != null)
            {
                inventoryController.HideInventory();
            }
        }
    }

    public bool IsShopOpen()
    {
        return isShopOpen;
    }

    private void HandlePurchaseRequest(ItemSO item, int price)
    {
        if (player == null || playerInventory == null)
        {

            return;
        }

        // Check if player has enough currency
        if (player.currency >= price)
        {
            // Add item to inventory
            int remainder = playerInventory.AddItem(item, 1);

            if (remainder == 0)
            {
                // Successfully added to inventory
                player.currency -= price;
                
                // Update shop UI to reflect new currency
                shopUI.UpdateAffordability(player.currency, shopInventory.GetShopItems());

                // Play success sound
                if (audioSource != null && purchaseSuccessClip != null)
                {
                    audioSource.PlayOneShot(purchaseSuccessClip);
                }
            }
            else
            {
                // Inventory full
                PlayFailSound();
            }
        }
        else
        {
            // Not enough currency
            PlayFailSound();
        }
    }

    private void PlayFailSound()
    {
        if (audioSource != null && purchaseFailClip != null)
        {
            audioSource.PlayOneShot(purchaseFailClip);
        }
    }

    private void Update()
    {
        // Allow closing shop with ESC (K is handled by MerchantNPC)
        if (isShopOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseShop();
        }
    }
}
