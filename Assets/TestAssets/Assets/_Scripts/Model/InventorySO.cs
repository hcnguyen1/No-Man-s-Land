using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// the inventory itself needs to know the current state of an item as well as other scriptable object related methods.
// have another object class called InventoryItem so i added an extra m at the end. InventoryItem[m]

[CreateAssetMenu]
public class InventorySO : ScriptableObject
{
    [SerializeField]
    private List<InventoryItemm> inventoryItems; // references the items on the list 

    [field: SerializeField]
    public int Size { get; private set; } = 10;

    public void Initialize()
    {
        inventoryItems = new List<InventoryItemm>();

        for (int i = 0; i < Size; i++)
        {
            inventoryItems.Add(InventoryItemm.GetEmptyItem());
        }
    }

    //InventoryItem item = new InventoryItem(); // the default will be null and 0 due to the method. 
    public void AddItem(ItemSO item, int quantity)
    {
        for (int i = 0; i < inventoryItems.Count; i++)
        {
            if (inventoryItems[i].IsEmpty)
            {
                inventoryItems[i] = new InventoryItemm
                {
                    item = item,
                    quantity = quantity
                };
            }
        }
    }

    public Dictionary<int, InventoryItemm> GetCurrentInventoryState() // we can only update the item if it is in the dictionary. 
    {
        Dictionary<int, InventoryItemm> returnValue = new Dictionary<int, InventoryItemm>();
        for (int i = 0; i < inventoryItems.Count; i++)
        {
            if (inventoryItems[i].IsEmpty) // if we cannot connect, we cannot modify values in the list
                continue;
            returnValue[i] = inventoryItems[i];
        }
        return returnValue; // returns the modified value
    }

    public InventoryItemm GetItemAt(int itemIndex) // when we are clicking on the item inside of the inventory we want to return the index
    {
        return inventoryItems[itemIndex];
    }
}

[Serializable]
public struct InventoryItemm // we can assign the value of the newly duped item and change its quantity, cannot modify original ItemSO
{
    public int quantity;
    public ItemSO item;
    public bool IsEmpty => item == null;

    public InventoryItemm ChangeQuantity(int newQuantity)
    {
        return new InventoryItemm
        {
            item = this.item,
            quantity = newQuantity,
        };
    }

    public static InventoryItemm GetEmptyItem() => new InventoryItemm
    {
        item = null,
        quantity = 0,
    };
}