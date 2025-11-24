using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inventory.UI;
using Inventory.Model;
using System.Text;

namespace Inventory // this creates its own kind of import settings that can only make it be used by those 
{// that call this namepace via ->  using Inventory;
    public class InventoryController : MonoBehaviour
    {

        [SerializeField]
        private InventoryPage inventoryUI;

        [SerializeField]
        private InventorySO inventoryData;

        [SerializeField]
        private AudioClip dropItemClip;

        [SerializeField]
        private AudioSource audioSource;

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
            string description = PrepareDescription(inventoryItem);
            inventoryUI.UpdateDescription(itemIndex, item.ItemImage, item.Name, description);
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

        private void HandleItemActionRequest(int itemIndex) // this will let us show options when right clicking 
        {
            InventoryEntry inventoryItem = inventoryData.GetItemAt(itemIndex);
            if (inventoryItem.IsEmpty)
                return;

            // lets us interact with the item, but needs to destroy object first if theres no slots. only applicable when calling action related methods
            IItemAction itemAction = inventoryItem.item as IItemAction;
            if (itemAction != null)
            {
                inventoryUI.ShowItemAction(itemIndex);
                inventoryUI.AddAction(itemAction.ActionName, () => PerformAction(itemIndex)); // this will trigger itemActionPanel
            }

            // destroys the item when removing or equipping
            IDestroyableItem destroyableItem = inventoryItem.item as IDestroyableItem;
            if (destroyableItem != null)
            {
                inventoryUI.AddAction("Drop", () => DropItem(itemIndex, inventoryItem.quantity));
            }
        }

        private void DropItem(int itemIndex, int quantity)
        {
            inventoryData.RemoveItem(itemIndex, quantity); // removes item from the inventory data
            inventoryUI.ResetSelection(); // then deselects the item. 
            audioSource.PlayOneShot(dropItemClip);
        }

        public string PrepareDescription(InventoryEntry inventoryItem)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(inventoryItem.item.Description);
            sb.AppendLine();

            for (int i = 0; i < inventoryItem.itemState.Count; i++) // this is the durability function when implementing durability, it is the current durability divided by the default amount. 
            {
                sb.Append($"{inventoryItem.itemState[i].itemParameter.ParameterName} " + $": {inventoryItem.itemState[i].value} / {inventoryItem.item.DefaultParametersList[i].value}");
                sb.AppendLine();
            }
            return sb.ToString();
        }

        public void PerformAction(int itemIndex)
        {
            InventoryEntry inventoryItem = inventoryData.GetItemAt(itemIndex);
            if (inventoryItem.IsEmpty)
                return;

            // destroys the item when removing or equipping
            IDestroyableItem destroyableItem = inventoryItem.item as IDestroyableItem;
            if (destroyableItem != null)
            {
                inventoryData.RemoveItem(itemIndex, 1);
            }

            // lets us interact with the item, but needs to destroy object first if theres no slots. 
            IItemAction itemAction = inventoryItem.item as IItemAction;
            if (itemAction != null)
            {
                itemAction.PerformAction(gameObject, inventoryItem.itemState);
                audioSource.PlayOneShot(itemAction.actionSFX);
                if (inventoryData.GetItemAt(itemIndex).IsEmpty)
                    inventoryUI.ResetSelection();
            }
        }
    }
}
