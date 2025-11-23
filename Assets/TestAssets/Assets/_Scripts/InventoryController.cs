using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inventory.UI;
using Inventory.Model;

namespace Inventory // this creates its own kind of import settings that can only make it be used by those 
{// that call this namepace via ->  using Inventory;
    public class InventoryController : MonoBehaviour
    {

        [SerializeField]
        private InventoryPage inventoryUI;

        [SerializeField]
        private InventorySO inventoryData;

        public List<InventoryEntry> initialItems = new List<InventoryEntry>(); // for the start inventoryData

        private void Start()
        {
            PrepareUI(); // we get the UI ready to handle all requests and movements/drag
            PrepareInventoryData(); // we get the inventory data ready
        }

        private void PrepareInventoryData()
        { // we use this to initialize our inventory data.
            inventoryData.Initialize();
            inventoryData.OnInventoryUpdated += UpdateInventoryUI;
            foreach (InventoryEntry item in initialItems)
            {
                if (item.IsEmpty)
                    continue;
                inventoryData.AddItem(item);
            }
        }

        private void UpdateInventoryUI(Dictionary<int, InventoryEntry> inventoryState)
        {
            inventoryUI.ResetAllItems(); // resets all data and items
            foreach (var item in inventoryState)
            {
                inventoryUI.UpdateData(item.Key, item.Value.item.ItemImage, item.Value.quantity);
            }
        }

        private void PrepareUI()
        {
            inventoryUI.IntializeInventoryUI(inventoryData.Size);
            this.inventoryUI.OnDescriptionRequested += HandleDescriptionRequest;
            this.inventoryUI.OnSwapItems += HandleSwapItems;
            this.inventoryUI.OnStartDragging += HandleDragging;
            this.inventoryUI.OnItemActionRequested += HandleItemActionRequest;
        }
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                if (inventoryUI.isActiveAndEnabled == false)
                {
                    inventoryUI.Show();
                    foreach (var item in inventoryData.GetCurrentInventoryState()) // use the inventory state and get data from there.
                    {
                        inventoryUI.UpdateData(item.Key, item.Value.item.ItemImage, item.Value.quantity);
                    }
                }
                else
                {
                    inventoryUI.Hide();
                }
            }
        }

        private void HandleDescriptionRequest(int itemIndex) // handles the description when selecting item
        {
            InventoryEntry inventoryItem = inventoryData.GetItemAt(itemIndex);
            if (inventoryItem.IsEmpty)
            {
                inventoryUI.ResetSelection();
                return;
            }
            ItemSO item = inventoryItem.item;
            inventoryUI.UpdateDescription(itemIndex, item.ItemImage, item.Name, item.Description);
        }

        private void HandleSwapItems(int itemIndex1, int itemIndex2) // handles 2 different items to swap
        {
            inventoryData.SwapItems(itemIndex1, itemIndex2);
        }

        private void HandleDragging(int itemIndex) // helps with getting information while performing drag
        {
            InventoryEntry inventoryItem = inventoryData.GetItemAt(itemIndex);
            if (inventoryItem.IsEmpty)
                return;
            inventoryUI.CreateDraggedItem(inventoryItem.item.ItemImage, inventoryItem.quantity);
        }

        private void HandleItemActionRequest(int itemIndex)
        {

        }
    }
}
