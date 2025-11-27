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
    private CraftingSystem craftingSystem;
    private Collider2D benchCollider;
    private InventoryController inventoryController;
    private CraftingUI craftingUIScript;

    void Awake()
    {
        Debug.Log("CraftingBench Awake called!");
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
        
        Debug.Log($"CraftingBench Start: inventoryController = {(inventoryController != null ? "found" : "NOT FOUND")}");
        Debug.Log($"CraftingBench Start: craftingUIScript = {(craftingUIScript != null ? "found" : "NOT FOUND")}");
        Debug.Log($"CraftingBench Start: craftingUI = {(craftingUI != null ? craftingUI.name : "NOT ASSIGNED")}");
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
                if (craftingUI != null && craftingUI.activeSelf)
                {
                    tabManager.CloseCraftingTab();
                }
                else
                {
                    tabManager.OpenCraftingTab();
                    if (craftingUIScript != null)
                        craftingUIScript.SetCraftingBench(this);
                }
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
        if (recipes.Count == 0)
        {
            Debug.LogWarning("CraftingBench has no recipes assigned!");
        }
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
