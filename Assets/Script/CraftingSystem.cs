using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingSystem
{
    public const int GRID_SIZE = 3;

    private ItemSO[,] itemArray;

    // default constructor
    public CraftingSystem()
    {
        itemArray = new ItemSO[GRID_SIZE, GRID_SIZE];
    }

    private bool isEmpty(int x, int y) // checks if certain position is empty 
    {
        return itemArray[x, y] == null;
    }

    private ItemSO GetItem(int x, int y) // get the items position
    {
        return itemArray[x, y];
    }

    private void SetItem(ItemSO item, int x, int y) // set item in that position
    {
        itemArray[x, y] = item;
    }

    private void IncreaseItemAmount(int x, int y) // increase amount 
    {
        GetItem(x, y).amountToChangeStat++;
    }

    private void DecreaseItemAmount(int x, int y) // decrease amount
    {
        GetItem(x, y).amountToChangeStat--;
    }

    private void removeItem(int x, int y) // removes the item 
    {
        SetItem(null, x, y);
    }

    private bool TryAddItem(ItemSO item, int x, int y) // attempts to add item if not full 
    {
        if (isEmpty(x, y))
        {
            SetItem(item, x, y);
            return true;
        }
        else
        {
            if (item.itemName == GetItem(x, y).itemName)
            {
                IncreaseItemAmount(x, y);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

