using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftingUIController : MonoBehaviour
{
    [Header("References")]
    public CraftingBench bench;
    public InventoryManager inventoryManager;
    public CraftingRecipe[] recipes;

    [Header("UI Elements")]
    public TMP_Text craftableRecipeText;
    public Image craftableTableImage;
    public Button craftButton;
    public GameObject craftingMenu;

    [Header("Crafting Grid UI")]
    public Image[] craftingGridImages; // Array of 9 Images for 3x3 grid
    public TMP_Text[] craftingGridQuantityTexts; // Array of 9 quantity texts

    private CraftingRecipe selectedRecipe;
    private ItemSlot draggedItemSlot; // Track which slot is being dragged from
    
    void Start()
    {
        if (bench == null)
            bench = FindObjectOfType<CraftingBench>();
        if (inventoryManager == null)
            inventoryManager = FindObjectOfType<InventoryManager>();

        if (craftingMenu != null)
            craftingMenu.SetActive(false);

        // Subscribe to ItemSlot drag/drop events
        SubscribeToItemSlots();

        UpdateCraftable();
    }

    void Update()
    {
        // If the crafting menu is visible, refresh what can be crafted
        if (craftingMenu != null && craftingMenu.activeSelf)
        {
            UpdateCraftable();
            RefreshGridUI();
        }
    }

    private void SubscribeToItemSlots()
    {
        // Find all ItemSlot components and subscribe to their events
        ItemSlot[] allSlots = inventoryManager.itemSlot;
        if (allSlots != null)
        {
            foreach (var slot in allSlots)
            {
                if (slot != null)
                {
                    slot.OnItemBeginDrag += OnInventoryItemBeginDrag;
                    slot.OnItemDropped += OnInventoryItemDropped;
                    slot.OnItemEndDrag += OnInventoryItemEndDrag;
                }
            }
        }
    }

    private void OnInventoryItemBeginDrag(InventoryManager inv)
    {
        // Find the selected slot
        draggedItemSlot = FindSelectedItemSlot();
        if (draggedItemSlot == null) return;
        // Visual feedback could go here
    }

    private void OnInventoryItemDropped(InventoryManager inv)
    {
        // Check if mouse is over a crafting grid cell
        // For now, we'll use a simple approach: click on a grid cell button to place the item
    }

    private void OnInventoryItemEndDrag(InventoryManager inv)
    {
        draggedItemSlot = null;
    }

    private ItemSlot FindSelectedItemSlot()
    {
        ItemSlot[] allSlots = inventoryManager.itemSlot;
        if (allSlots != null)
        {
            foreach (var slot in allSlots)
            {
                if (slot != null && slot.thisItemSelected)
                    return slot;
            }
        }
        return null;
    }

    // Called by grid cell buttons to place item into crafting grid
    public void PlaceItemIntoGrid(int cellIndex)
    {
        if (bench == null) return;
        var system = bench.GetCraftingSystem();
        if (system == null) return;

        var slot = FindSelectedItemSlot();
        if (slot == null || string.IsNullOrEmpty(slot.itemName) || slot.quantity <= 0)
            return;

        // Convert cellIndex (0-8) to x, y (0-2, 0-2)
        int x = cellIndex % 3;
        int y = cellIndex / 3;

        // Find the ItemSO for this item
        ItemSO itemSO = FindItemSO(slot.itemName);
        if (itemSO == null) return;

        // Try to place 1 item into the grid cell
        bool success = system.TryAddToCell(itemSO, 1, x, y);
        if (success)
        {
            // Decrease inventory slot quantity
            slot.AddQuantity(-1);
            if (slot.quantity <= 0)
            {
                slot.ClearSlot();
            }
        }

        RefreshGridUI();
    }

    // Called by grid cell buttons to remove item from grid
    public void RemoveItemFromGrid(int cellIndex)
    {
        if (bench == null) return;
        var system = bench.GetCraftingSystem();
        if (system == null) return;

        int x = cellIndex % 3;
        int y = cellIndex / 3;

        var cell = system.GetCell(x, y);
        if (cell.IsEmpty) return;

        // Return 1 item to inventory
        if (inventoryManager != null && cell.item != null)
        {
            inventoryManager.AddItem(cell.item.itemName, 1, cell.item.icon, cell.item.itemDescription);
        }

        // Decrease grid quantity
        system.SetCell(cell.item, cell.quantity - 1, x, y);
        if (cell.quantity - 1 <= 0)
        {
            system.ClearCell(x, y);
        }

        RefreshGridUI();
    }

    public void UpdateCraftable()
    {
        selectedRecipe = null;
        craftableRecipeText.text = "";
        craftableTableImage.sprite = null;
        craftButton.interactable = false;

        if (bench == null) return;
        var system = bench.GetCraftingSystem();
        if (system == null || recipes == null) return;

        foreach (var r in recipes)
        {
            if (system.MatchesRecipe(r))
            {
                selectedRecipe = r;
                craftableRecipeText.text = r.result.itemName + " x" + r.resultQuantity;
                if (r.result.icon != null)
                    craftableTableImage.sprite = r.result.icon;
                craftButton.interactable = true;
                return;
            }
        }
    }

    public void OnCraftButton()
    {
        if (selectedRecipe == null) return;
        if (bench == null) return;

        var system = bench.GetCraftingSystem();
        if (system == null) return;

        bool applied = system.ApplyRecipe(selectedRecipe);
        if (!applied) return;

        // Add crafted result to player inventory
        if (inventoryManager != null && selectedRecipe.result != null)
        {
            inventoryManager.AddItem(selectedRecipe.result.itemName, selectedRecipe.resultQuantity, selectedRecipe.result.icon, selectedRecipe.result.itemDescription);
        }

        RefreshGridUI();
        UpdateCraftable();
    }

    private void RefreshGridUI()
    {
        if (bench == null || craftingGridImages == null || craftingGridImages.Length == 0)
            return;

        var system = bench.GetCraftingSystem();
        if (system == null) return;

        for (int i = 0; i < 9 && i < craftingGridImages.Length; i++)
        {
            int x = i % 3;
            int y = i / 3;

            var cell = system.GetCell(x, y);
            if (cell.IsEmpty)
            {
                craftingGridImages[i].sprite = null;
                if (craftingGridQuantityTexts != null && i < craftingGridQuantityTexts.Length)
                {
                    craftingGridQuantityTexts[i].text = "";
                    craftingGridQuantityTexts[i].enabled = false;
                }
            }
            else
            {
                craftingGridImages[i].sprite = cell.item.icon;
                if (craftingGridQuantityTexts != null && i < craftingGridQuantityTexts.Length)
                {
                    craftingGridQuantityTexts[i].text = cell.quantity.ToString();
                    craftingGridQuantityTexts[i].enabled = (cell.quantity > 0);
                }
            }
        }
    }

    private ItemSO FindItemSO(string itemName)
    {
        if (inventoryManager == null || inventoryManager.itemSOs == null)
            return null;

        foreach (var so in inventoryManager.itemSOs)
        {
            if (so != null && so.itemName == itemName)
                return so;
        }
        return null;
    }

    public void ToggleMenu()
    {
        if (craftingMenu == null) return;
        craftingMenu.SetActive(!craftingMenu.activeSelf);
        if (craftingMenu.activeSelf)
            UpdateCraftable();
    }
}
