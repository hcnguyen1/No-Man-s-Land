using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inventory.Model;

public class CraftingSystem
{
    public const int GRID_SIZE = 3;

    public struct Cell
    {
        public ItemSO item;
        public int quantity;
        public bool IsEmpty => item == null || quantity <= 0;
    }

    private Cell[,] grid;

    public CraftingSystem()
    {
        grid = new Cell[GRID_SIZE, GRID_SIZE];
    }

    public Cell GetCell(int x, int y)
    {
        return grid[x, y];
    }

    public void SetCell(ItemSO item, int quantity, int x, int y)
    {
        grid[x, y].item = item;
        grid[x, y].quantity = quantity;
    }

    public void ClearCell(int x, int y)
    {
        grid[x, y].item = null;
        grid[x, y].quantity = 0;
    }

    public bool TryAddToCell(ItemSO item, int amount, int x, int y)
    {
        var c = grid[x, y];
        if (c.IsEmpty)
        {
            grid[x, y].item = item;
            grid[x, y].quantity = amount;
            return true;
        }

        if (c.item == item)
        {
            grid[x, y].quantity += amount;
            return true;
        }

        return false;
    }

    // Recipe-based crafting methods
    public bool CanCraft(CraftingRecipe recipe, InventorySO inventory)
    {
        return HasEnoughIngredients(recipe, inventory);
    }

    public bool TryCraft(CraftingRecipe recipe, InventorySO inventory)
    {
        if (!CanCraft(recipe, inventory))
            return false;

        RemoveIngredients(recipe, inventory);
        inventory.AddItem(new InventoryEntry { item = recipe.result, quantity = recipe.resultQuantity });
        return true;
    }

    public List<string> GetMissingIngredients(CraftingRecipe recipe, InventorySO inventory)
    {
        List<string> missing = new List<string>();

        foreach (var ingredient in recipe.ingredients)
        {
            int count = CountItemInInventory(ingredient.item, inventory);
            if (count < ingredient.amount)
            {
                missing.Add($"{ingredient.item.Name} (need {ingredient.amount}, have {count})");
            }
        }

        return missing;
    }

    private bool HasEnoughIngredients(CraftingRecipe recipe, InventorySO inventory)
    {
        foreach (var ingredient in recipe.ingredients)
        {
            int count = CountItemInInventory(ingredient.item, inventory);
            if (count < ingredient.amount)
                return false;
        }
        return true;
    }

    private int CountItemInInventory(ItemSO item, InventorySO inventory)
    {
        int count = 0;
        var state = inventory.GetCurrentInventoryState();
        foreach (var entry in state.Values)
        {
            if (entry.item == item)
                count += entry.quantity;
        }
        return count;
    }

    private void RemoveIngredients(CraftingRecipe recipe, InventorySO inventory)
    {
        foreach (var ingredient in recipe.ingredients)
        {
            int remaining = ingredient.amount;
            var state = inventory.GetCurrentInventoryState();

            foreach (var slot in state)
            {
                if (slot.Value.item == ingredient.item && remaining > 0)
                {
                    int removeAmount = Mathf.Min(remaining, slot.Value.quantity);
                    inventory.RemoveItem(slot.Key, removeAmount);
                    remaining -= removeAmount;
                }
            }
        }
    }
}

