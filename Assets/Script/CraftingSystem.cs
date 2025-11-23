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
}

