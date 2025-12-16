using UnityEngine;
using Inventory;
using Inventory.Model;


// This class allows one to interact with a Campfire UI once they are within proximity and 
// if the meat is inside the player's inventory via inventoryController, we then can press a key, F being in this case, to cook the rawmeat.
// The rawmeat item is swapped with the cooked meat item and the audio will play a sound queue to cook. 
[RequireComponent(typeof(Collider2D))]
public class Campfire : MonoBehaviour
{
    [SerializeField]
    private ItemSO rawMeat; // Assign RawMeat ItemSO

    [SerializeField]
    private ItemSO cookedMeat; // Assign CookedMeat ItemSO

    [SerializeField]
    private GameObject cookPrompt; // Optional: "Press F to cook meat" text

    [SerializeField]
    private AudioClip cookSound; // Optional: cooking sound

    private bool playerNearCampfire = false;
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
        if (cookPrompt != null)
        {
            cookPrompt.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && playerNearCampfire)
        {
            TryCookMeat();
        }
    }

    private void TryCookMeat()
    {
        if (inventoryController == null || rawMeat == null || cookedMeat == null)
        {
            return;
        }

        InventorySO inventory = inventoryController.inventoryData;
        HotbarSO hotbar = inventoryController.HotbarData;

        // First, check main inventory for raw meat
        int rawMeatIndex = -1;
        bool foundInInventory = false;
        
        for (int i = 0; i < inventory.Size; i++)
        {
            var entry = inventory.GetItemAt(i);
            if (!entry.IsEmpty && entry.item == rawMeat)
            {
                rawMeatIndex = i;
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
                if (!entry.IsEmpty && entry.item == rawMeat)
                {
                    rawMeatIndex = i;
                    foundInInventory = false; // Found in hotbar
                    break;
                }
            }
        }

        if (rawMeatIndex >= 0)
        {
            // Remove 1 raw meat from the correct location
            if (foundInInventory)
            {
                inventory.RemoveItem(rawMeatIndex, 1);
            }
            else
            {
                hotbar.RemoveItem(rawMeatIndex, 1);
            }

            // Try to add cooked meat to inventory first
            int remainder = inventory.AddItem(cookedMeat, 1);
            
            // If inventory full, try hotbar
            if (remainder > 0 && hotbar != null)
            {
                remainder = hotbar.AddItem(cookedMeat, 1);
            }

            if (remainder == 0)
            {
                // Successfully cooked
                // Play sound
                if (audioSource != null && cookSound != null)
                {
                    audioSource.PlayOneShot(cookSound);
                }
            }
            else
            {
                // Both full, give back raw meat
                if (foundInInventory)
                {
                    inventory.AddItem(rawMeat, 1);
                }
                else
                {
                    hotbar.AddItem(rawMeat, 1);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerNearCampfire = true;

            if (cookPrompt != null)
            {
                cookPrompt.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerNearCampfire = false;

            if (cookPrompt != null)
            {
                cookPrompt.SetActive(false);
            }
        }
    }
}
