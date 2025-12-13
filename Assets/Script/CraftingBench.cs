using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inventory.Model;
using Inventory;

[RequireComponent(typeof(Collider2D))]
public class CraftingBench : MonoBehaviour
{
    [Tooltip("Assign the crafting UI GameObject (CraftingMenu panel)")]
    public GameObject craftingUI;

    [SerializeField]
    private List<CraftingRecipe> recipes = new List<CraftingRecipe>();

    private bool playerNearBench = false;
    private bool isCraftingOpen = false;
    private CraftingSystem craftingSystem;
    private Collider2D benchCollider;
    private InventoryController inventoryController;
    private CraftingUI craftingUIScript;

    void Awake()
    {
        craftingSystem = new CraftingSystem();
        benchCollider = GetComponent<Collider2D>();
        if (benchCollider != null)
            benchCollider.isTrigger = true;
    }

    void Start()
    {
        inventoryController = FindObjectOfType<InventoryController>();
        craftingUIScript = FindObjectOfType<CraftingUI>();
        
        // If not found globally, try to find it on the craftingUI GameObject
        if (craftingUIScript == null && craftingUI != null)
        {
            craftingUIScript = craftingUI.GetComponent<CraftingUI>();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearBench = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearBench = false;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K) && playerNearBench)
        {
            if (isCraftingOpen)
            {
                // Close crafting
                CloseCrafting();
            }
            else
            {
                // Open crafting
                OpenCrafting();
            }
        }

        // Close crafting if player walks away
        if (isCraftingOpen && !playerNearBench)
        {
            CloseCrafting();
        }
    }

    private void OpenCrafting()
    {
        if (craftingUI != null)
        {
            craftingUI.SetActive(true);
            isCraftingOpen = true;

            // Set the bench reference
            if (craftingUIScript == null)
            {
                craftingUIScript = craftingUI.GetComponent<CraftingUI>();
            }

            if (craftingUIScript != null)
            {
                craftingUIScript.SetCraftingBench(this);
            }

            // Show inventory
            if (inventoryController != null)
            {
                inventoryController.ShowInventory();
            }

            Debug.Log("Crafting opened!");
        }
    }

    private void CloseCrafting()
    {
        if (craftingUI != null)
        {
            craftingUI.SetActive(false);
            isCraftingOpen = false;

            // Hide inventory
            if (inventoryController != null)
            {
                inventoryController.HideInventory();
            }

            Debug.Log("Crafting closed!");
        }
    }

    public CraftingSystem GetCraftingSystem()
    {
        return craftingSystem;
    }

    public bool IsPlayerNearby()
    {
        return playerNearBench;
    }

    public List<CraftingRecipe> GetAvailableRecipes()
    {
        return recipes;
    }

    public bool CanCraft(CraftingRecipe recipe)
    {
        if (inventoryController == null)
            return false;

        InventorySO inventory = inventoryController.inventoryData;
        return craftingSystem.CanCraft(recipe, inventory);
    }

    public List<string> GetMissingIngredients(CraftingRecipe recipe)
    {
        if (inventoryController == null)
            return new List<string>();

        InventorySO inventory = inventoryController.inventoryData;
        return craftingSystem.GetMissingIngredients(recipe, inventory);
    }

    public bool TryCraft(CraftingRecipe recipe)
    {
        if (inventoryController == null)
            return false;

        InventorySO inventory = inventoryController.inventoryData;
        bool success = craftingSystem.TryCraft(recipe, inventory);
        return success;
    }
}
