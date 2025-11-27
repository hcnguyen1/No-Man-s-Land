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

    private bool playerNearby = false;
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
            playerNearby = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            // TabManager handles closing the crafting menu via Player.canOpenCraftingMenu
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K) && playerNearby)
        {
            TabManager tabManager = FindObjectOfType<TabManager>();
            if (tabManager != null)
            {
                if (isCraftingOpen)
                {
                    // Close crafting
                    tabManager.CloseCraftingTab();
                    isCraftingOpen = false;
                }
                else
                {
                    // First, open crafting menu to activate the GameObject
                    tabManager.OpenCraftingTab();
                    isCraftingOpen = true;
                    
                    // THEN find CraftingUI (now that menu is active) and set the bench
                    if (craftingUIScript == null)
                        craftingUIScript = FindObjectOfType<CraftingUI>();
                    
                    // Set crafting bench AFTER menu is active so coroutines work
                    if (craftingUIScript != null)
                        craftingUIScript.SetCraftingBench(this);
                }
            }
        }
        
        // Close crafting menu only if player walks away (not when opening)
        if (isCraftingOpen && !playerNearby)
        {
            TabManager tabManager = FindObjectOfType<TabManager>();
            if (tabManager != null)
            {
                tabManager.CloseCraftingTab();
                isCraftingOpen = false;
            }
        }
    }

    public CraftingSystem GetCraftingSystem()
    {
        return craftingSystem;
    }

    public bool IsPlayerNearby()
    {
        return playerNearby;
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
