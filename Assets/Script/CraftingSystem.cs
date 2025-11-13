using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // default constructor
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

    // Try to add amount to a cell. Returns true if added (merged or placed), false if a different item occupies the cell
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

    // Check whether grid matches the shaped recipe exactly
    public bool MatchesRecipe(CraftingRecipe recipe)
    {
        if (recipe == null || recipe.ingredients == null) return false;

        bool[,] hasIngredient = new bool[GRID_SIZE, GRID_SIZE];
        foreach (var ing in recipe.ingredients)
        {
            if (ing.x < 0 || ing.x >= GRID_SIZE || ing.y < 0 || ing.y >= GRID_SIZE)
                return false;

            var c = GetCell(ing.x, ing.y);
            if (c.item == null) return false;
            if (c.item != ing.item) return false;
            if (c.quantity < ing.amount) return false;
            hasIngredient[ing.x, ing.y] = true;
        }

        // ensure cells not mentioned in recipe are empty
        for (int x = 0; x < GRID_SIZE; x++)
        {
            for (int y = 0; y < GRID_SIZE; y++)
            {
                if (!hasIngredient[x, y])
                {
                    var c = GetCell(x, y);
                    if (!c.IsEmpty) return false;
                }
            }
        }

        return true;
    }

    // Apply recipe: consumes ingredients from the grid
    public bool ApplyRecipe(CraftingRecipe recipe)
    {
        if (!MatchesRecipe(recipe)) return false;

        foreach (var ing in recipe.ingredients)
        {
            var c = grid[ing.x, ing.y];
            c.quantity -= ing.amount;
            if (c.quantity <= 0)
            {
                c.item = null;
                c.quantity = 0;
            }
            grid[ing.x, ing.y] = c;
        }

        return true;
    }
}

