using UnityEngine;
using TMPro;

public class MerchantNPC : MonoBehaviour
{
    // This class checks if the player is near the proximity of the merchant and therefore allow the merchant to 
    // be interactable. If the Player is indeed nearby, we can trade by pressing F.
    [SerializeField]
    private ShopController shopController;

    [SerializeField]
    private GameObject interactPrompt; // UI prompt "Press F to Trade"

    private bool playerNearShop = false;
    private bool isShopOpen = false;

    private void Start()
    {
        if (interactPrompt != null)
        {
            interactPrompt.SetActive(false);
        }

        if (shopController == null)
        {
            shopController = FindObjectOfType<ShopController>();
        }
    }

    private void Update()
    {
        // Press F to open/close shop (like crafting bench)
        if (Input.GetKeyDown(KeyCode.F) && playerNearShop)
        {
            if (shopController != null)
            {
                if (isShopOpen)
                {
                    // Close shop
                    shopController.CloseShop();
                    isShopOpen = false;
                    
                    // Show prompt again if player still near
                    if (interactPrompt != null && playerNearShop)
                    {
                        interactPrompt.SetActive(true);
                    }
                }
                else
                {
                    // Open shop (this automatically opens inventory too)
                    shopController.OpenShop();
                    isShopOpen = true;
                    
                    // Hide prompt when shop opens
                    if (interactPrompt != null)
                    {
                        interactPrompt.SetActive(false);
                    }
                }
            }
        }

        // Close shop if player walks away
        if (isShopOpen && !playerNearShop)
        {
            if (shopController != null)
            {
                shopController.CloseShop();
                isShopOpen = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerNearShop = true;
            
            // Only show prompt if shop is not already open
            if (interactPrompt != null && !isShopOpen)
            {
                interactPrompt.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerNearShop = false;
            
            if (interactPrompt != null)
            {
                interactPrompt.SetActive(false);
            }
        }
    }
}
