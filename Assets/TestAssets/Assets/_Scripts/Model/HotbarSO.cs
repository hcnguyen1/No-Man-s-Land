using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace Inventory.Model
{
    [CreateAssetMenu]
    public class HotbarSO : ScriptableObject
    {
        [SerializeField]
        private List<InventoryEntry> hotbarItems; // references the items on the hotbar

        [field: SerializeField]
        public int Size { get; private set; } = 8; // Hotbar has 8 slots

        public event Action<Dictionary<int, InventoryEntry>> OnHotbarUpdated; // can use this to inform about any changes to the hotbar.

        public void Initialize()
        {
            hotbarItems = new List<InventoryEntry>();

            for (int i = 0; i < Size; i++)
            {
                hotbarItems.Add(InventoryEntry.GetEmptyItem());
            }
        }

        public int AddItem(ItemSO item, int quantity, List<ItemParameter> itemState = null) // also can see if its stackable 
        {
            if (item.IsStackable == false) // while it cannot be stacked
            {
                for (int i = 0; i < hotbarItems.Count; i++)
                {
                    while (quantity > 0 && IsHotbarFull() == false) // while not full 
                    {
                        quantity -= AddItemToFirstFreeSlot(item, 1, itemState);
                    }
                    InformAboutChange();
                    return quantity;
                }
            } // else stack
            quantity = AddStackableItem(item, quantity); // stack the item, then inform class about change
            InformAboutChange();
            return quantity;
        }

        private int AddItemToFirstFreeSlot(ItemSO item, int quantity, List<ItemParameter> itemState = null)
        {
            InventoryEntry newItem = new InventoryEntry
            {
                item = item,
                quantity = quantity,
                itemState = new List<ItemParameter>(itemState == null ? item.DefaultParametersList : itemState)
            };

            for (int i = 0; i < hotbarItems.Count; i++)
            {
                if (hotbarItems[i].IsEmpty)
                {
                    hotbarItems[i] = newItem;
                    return quantity;
                }
            }
            return 0;
        }

        //  checks if hotbar is full by searching where the empty itself might be empty, and if its false then it means it is full
        private bool IsHotbarFull() => hotbarItems.Where(item => item.IsEmpty).Any() == false;

        private int AddStackableItem(ItemSO item, int quantity)
        {
            for (int i = 0; i < hotbarItems.Count; i++)
            {
                if (hotbarItems[i].IsEmpty) // if its empty, continue
                {
                    AddItemToFirstFreeSlot(item, quantity);
                    return 0;
                }

                if (hotbarItems[i].item == item) // if same item
                {
                    int amountPossibleToTake = hotbarItems[i].item.MaxStackSize - hotbarItems[i].quantity;
                    if (amountPossibleToTake <= 0)
                        continue;

                    int quantityToAdd = Mathf.Clamp(quantity, 0, amountPossibleToTake);
                    InventoryEntry newItem = hotbarItems[i];
                    newItem.quantity += quantityToAdd;
                    hotbarItems[i] = newItem;

                    quantity -= quantityToAdd;

                    if (quantity <= 0)
                        return 0;
                }
            }
            return quantity;
        }

        public Dictionary<int, InventoryEntry> GetCurrentHotbarState()
        {
            Dictionary<int, InventoryEntry> returnValue = new Dictionary<int, InventoryEntry>();
            for (int i = 0; i < hotbarItems.Count; i++)
            {
                if (hotbarItems[i].IsEmpty) // if we cannot connect, we cannot modify values in the list
                    continue;
                returnValue[i] = hotbarItems[i];
            }
            return returnValue; // returns the modified value
        }

        public InventoryEntry GetItemAt(int itemIndex) // when we are clicking on the item inside of the hotbar we want to return the index
        {
            return hotbarItems[itemIndex];
        }

        public void SwapItems(int itemIndex1, int itemIndex2) // classic swapping using 2 ints.
        {
            InventoryEntry item1 = hotbarItems[itemIndex1]; // will be assigned to this class's struct 
            hotbarItems[itemIndex1] = hotbarItems[itemIndex2];
            hotbarItems[itemIndex2] = item1;
            InformAboutChange();
        }

        public void RemoveItem(int itemIndex, int quantity) // removes item from hotbar
        {
            InventoryEntry item = hotbarItems[itemIndex];
            item.quantity -= quantity;
            if (item.quantity <= 0)
            {
                item = InventoryEntry.GetEmptyItem();
            }
            hotbarItems[itemIndex] = item;
            InformAboutChange();
        }

        // Place an item at a specific slot (for cross-inventory transfers)
        public void AddItemAtSlot(int slotIndex, ItemSO item, int quantity, List<ItemParameter> itemState = null)
        {
            if (slotIndex < 0 || slotIndex >= hotbarItems.Count)
                return;

            InventoryEntry newItem = new InventoryEntry
            {
                item = item,
                quantity = quantity,
                itemState = new List<ItemParameter>(itemState == null ? item.DefaultParametersList : itemState)
            };

            hotbarItems[slotIndex] = newItem;
            InformAboutChange();
        }

        private void InformAboutChange() // we need to pass the dictionary to the hotbar UI
        {
            OnHotbarUpdated?.Invoke(GetCurrentHotbarState()); // can check dictionary about the hotbar data when called.
        }
    }
}
