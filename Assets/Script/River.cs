using UnityEngine;
using Inventory;
using Inventory.Model;


// The class functions like other gameobjects that the player can interact with.
// It allows the player to convert empty bottle into a water bottle if they are within the proximity of the river.
// it allow creates a prompt so that the player knows they are within range as well as tell them the interaction key.
// The audio source will play a clip when the action goes through. 
// lastly, it connects to the inventoryController so that the item will be swapped inside of the inventory.
[RequireComponent(typeof(Collider2D))]
public class River : MonoBehaviour
{
    [SerializeField]
    private ItemSO emptyBottle; // Assign Empty Bottle ItemSO

    [SerializeField]
    private ItemSO bottledWater; // Assign Bottled Water ItemSO

    [SerializeField]
    private GameObject interactPrompt; // Optional: "Press F to fill bottle" text

    [SerializeField]
    private AudioClip fillSound; // Optional: filling sound

    private bool playerNearRiver = false;
    private InventoryController inventoryController;
    private AudioSource audioSource;

    private void Start()
    {
        inventoryController = FindObjectOfType<InventoryController>();
        audioSource = GetComponent<AudioSource>();

        // Make sure collider is a trigger
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.isTrigger = true;
        }

        // Hide prompt initially
        if (interactPrompt != null)
        {
            interactPrompt.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && playerNearRiver)
        {
            TryFillBottle();
        }
    }

    private void TryFillBottle()
    {
        if (inventoryController == null || emptyBottle == null || bottledWater == null)
        {
            return;
        }

        InventorySO inventory = inventoryController.inventoryData;
        HotbarSO hotbar = inventoryController.HotbarData;

        // First, check main inventory for empty bottle
        int bottleIndex = -1;
        bool foundInInventory = false;
        
        for (int i = 0; i < inventory.Size; i++)
        {
            var entry = inventory.GetItemAt(i);
            if (!entry.IsEmpty && entry.item == emptyBottle)
            {
                bottleIndex = i;
                foundInInventory = true;
                break;
            }
        }

        // If not in inventory, check hotbar
        if (!foundInInventory && hotbar != null)
        {
            for (int i = 0; i < hotbar.Size; i++)
            {
                var entry = hotbar.GetItemAt(i);
                if (!entry.IsEmpty && entry.item == emptyBottle)
                {
                    bottleIndex = i;
                    foundInInventory = false; // Found in hotbar
                    break;
                }
            }
        }

        if (bottleIndex >= 0)
        {
            // Remove 1 empty bottle from the correct location
            if (foundInInventory)
            {
                inventory.RemoveItem(bottleIndex, 1);
            }
            else
            {
                hotbar.RemoveItem(bottleIndex, 1);
            }

            // Try to add bottled water to inventory first
            int remainder = inventory.AddItem(bottledWater, 1);
            
            // If inventory full, try hotbar
            if (remainder > 0 && hotbar != null)
            {
                remainder = hotbar.AddItem(bottledWater, 1);
            }

            if (remainder == 0)
            {
                // Successfully filled
                // Play sound
                if (audioSource != null && fillSound != null)
                {
                    audioSource.PlayOneShot(fillSound);
                }
            }
            else
            {
                // Both full, give back empty bottle
                if (foundInInventory)
                {
                    inventory.AddItem(emptyBottle, 1);
                }
                else
                {
                    hotbar.AddItem(emptyBottle, 1);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerNearRiver = true;
            if (interactPrompt != null)
            {
                interactPrompt.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerNearRiver = false;
            if (interactPrompt != null)
            {
                interactPrompt.SetActive(false);
            }
        }
    }
}
