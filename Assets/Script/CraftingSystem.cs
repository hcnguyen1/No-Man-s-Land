using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inventory.Model;

// crafting system makes sure the item can be crafted or if not, what the missing ingredients are.
// It works off a boolean and can check and update itself to see if the player continuously has enough ingredients before and after crafting.
public class CraftingSystem
{
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

