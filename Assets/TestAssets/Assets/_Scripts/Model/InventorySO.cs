using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

// the inventory itself needs to know the current state of an item as well as other scriptable object related methods.
// have another object class called InventoryItem so we renamed it to InventoryEntry to avoid confusion

namespace Inventory.Model
{
    [CreateAssetMenu]
    public class InventorySO : ScriptableObject
    {
        [SerializeField]
        private List<InventoryEntry> inventoryItems; // references the items on the list 

        [field: SerializeField]
        public int Size { get; private set; } = 10;

        public event Action<Dictionary<int, InventoryEntry>> OnInventoryUpdated; // can use this to inform about any changes to the inventory. 

        public void Initialize()
        {
            inventoryItems = new List<InventoryEntry>();

            for (int i = 0; i < Size; i++)
            {
                inventoryItems.Add(InventoryEntry.GetEmptyItem());
            }
        }

        //InventoryItem item = new InventoryItem(); // the default will be null and 0 due to the method. 
        public int AddItem(ItemSO item, int quantity) // also can see if its stackable 
        {
            if (item.IsStackable == false) // while it cannot be stacked
            {
                for (int i = 0; i < inventoryItems.Count; i++)
                {
                    while (quantity > 0 && IsInventoryFull() == false) // while not full 
                    {
                        quantity -= AddItemToFirstFreeSlot(item, 1);

                    }
                    InformAboutChange();
                    return quantity;
                }
            } // else stack
            quantity = AddStackableItem(item, quantity); // stack the item, then inform class about change
            InformAboutChange();
            return quantity;
        }

        private int AddItemToFirstFreeSlot(ItemSO item, int quantity)
        {
            InventoryEntry newItem = new InventoryEntry
            {
                item = item,
                quantity = quantity
            };

            for (int i = 0; i < inventoryItems.Count; i++)
            {
                if (inventoryItems[i].IsEmpty)
                {
                    inventoryItems[i] = newItem;
                    return quantity;
                }
            }
            return 0;
        }

        //  checks if inventory is full by searching where the empty itself might be empty, and if its false then it means it is full
        private bool IsInventoryFull() => inventoryItems.Where(item => item.IsEmpty).Any() == false;

        private int AddStackableItem(ItemSO item, int quantity)
        {
            for (int i = 0; i < inventoryItems.Count; i++)
            {
                if (inventoryItems[i].IsEmpty) // if its empty, continue
                    continue;
                if (inventoryItems[i].item.ID == item.ID) // if it matches the items id,
                {
                    int amountPossibleToTake = inventoryItems[i].item.MaxStackSize - inventoryItems[i].quantity;

                    if (quantity > amountPossibleToTake) //if its not greater than the amount. if it is, subtract the max stack size then add it in
                    {
                        inventoryItems[i] = inventoryItems[i].ChangeQuantity(inventoryItems[i].item.MaxStackSize);
                        quantity -= amountPossibleToTake;
                    }
                    else // you can stack it now
                    {
                        inventoryItems[i] = inventoryItems[i].ChangeQuantity(inventoryItems[i].quantity + quantity);
                        InformAboutChange();
                        return 0;
                    }
                }
            }
            while (quantity > 0 && IsInventoryFull() == false) // so if there is space 
            {
                int newQuantity = Mathf.Clamp(quantity, 0 , item.MaxStackSize);
                quantity -= newQuantity;
                AddItemToFirstFreeSlot(item, newQuantity);
            }
            return quantity; // if its 0, then 0, if not leave some items behind. 
        }

        public void AddItem(InventoryEntry item)
        {
            AddItem(item.item, item.quantity);
        }

        public Dictionary<int, InventoryEntry> GetCurrentInventoryState() // we can only update the item if it is in the dictionary. 
        {
            Dictionary<int, InventoryEntry> returnValue = new Dictionary<int, InventoryEntry>();
            for (int i = 0; i < inventoryItems.Count; i++)
            {
                if (inventoryItems[i].IsEmpty) // if we cannot connect, we cannot modify values in the list
                    continue;
                returnValue[i] = inventoryItems[i];
            }
            return returnValue; // returns the modified value
        }

        public InventoryEntry GetItemAt(int itemIndex) // when we are clicking on the item inside of the inventory we want to return the index
        {
            return inventoryItems[itemIndex];
        }

        public void SwapItems(int itemIndex1, int itemIndex2) // classic swapping using 2 ints.
        {
            InventoryEntry item1 = inventoryItems[itemIndex1]; // will be assigned to this class's struct 
            inventoryItems[itemIndex1] = inventoryItems[itemIndex2];
            inventoryItems[itemIndex2] = item1;
            InformAboutChange();
        }

        private void InformAboutChange() // we need to pass the dictionary to the inventoryController
        {
            OnInventoryUpdated?.Invoke(GetCurrentInventoryState()); // can check dictionary about the inventorydata when called.
        }
    }

    [Serializable]
    public struct InventoryEntry // we can assign the value of the newly duped item and change its quantity, cannot modify original ItemSO
    {
        public int quantity;
        public ItemSO item;
        public bool IsEmpty => item == null;

        public InventoryEntry ChangeQuantity(int newQuantity) // when item quantity changes
        {
            return new InventoryEntry
            {
                item = this.item,
                quantity = newQuantity,
            };
        }

        public static InventoryEntry GetEmptyItem() => new InventoryEntry // when theres no item, we set all physical parameters to empty or zero. 
        {
            item = null,
            quantity = 0,
        };
    }
}
