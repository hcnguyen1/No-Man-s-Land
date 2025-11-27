using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inventory.UI;
using Inventory.Model;
using System.Text;

namespace Inventory // this creates its own kind of import settings that can only make it be used by those 
{// that call this namepace via ->  using Inventory;

    public enum ItemSource
    {
        Inventory,
        Hotbar
    }

    public class InventoryController : MonoBehaviour
    {

        [SerializeField]
        private InventoryPage inventoryUI;

        [SerializeField]
        public InventorySO inventoryData;

        [SerializeField]
        private HotbarUI hotbarUI;

        [SerializeField]
        private HotbarSO hotbarData;

        [SerializeField]
        private AudioClip dropItemClip;

        [SerializeField]
        private AudioSource audioSource;

        public List<InventoryEntry> initialItems = new List<InventoryEntry>(); // for the start inventoryData

        // Cross-inventory transfer tracking
        private ItemSource currentDragSource = ItemSource.Inventory;
        private int currentDragSourceIndex = -1;

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

            // Hotbar data initialization
            hotbarData.Initialize();
            hotbarData.OnHotbarUpdated += UpdateHotbarUI;
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

            // Hotbar UI initialization
            hotbarUI.IntializeHotbarUI(hotbarData.Size);
            this.hotbarUI.OnDescriptionRequested += HandleHotbarDescriptionRequest;
            this.hotbarUI.OnSwapItems += HandleHotbarSwapItems;
            this.hotbarUI.OnStartDragging += HandleHotbarDragging;
            this.hotbarUI.OnItemActionRequested += HandleHotbarItemActionRequest;
            
            // Cross-inventory drop handlers
            this.inventoryUI.OnItemDroppedOn += HandleInventoryItemDropped;
            this.hotbarUI.OnItemDroppedOn += HandleHotbarItemDropped;
        }
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                if (!inventoryUI.gameObject.activeSelf)
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
            // Both items in same inventory - regular swap
            inventoryData.SwapItems(itemIndex1, itemIndex2);
        }

        private void HandleDragging(int itemIndex) // helps with getting information while performing drag
        {
            InventoryEntry inventoryItem = inventoryData.GetItemAt(itemIndex);
            if (inventoryItem.IsEmpty)
                return;
            
            currentDragSource = ItemSource.Inventory;
            currentDragSourceIndex = itemIndex;
            
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
                if (itemAction.actionSFX != null)
                {
                    AudioSource.PlayClipAtPoint(itemAction.actionSFX, transform.position);
                }
                
                if (inventoryData.GetItemAt(itemIndex).IsEmpty)
                    inventoryUI.ResetSelection();
            }
        }

        // ========== HOTBAR EVENT HANDLERS ==========

        private void UpdateHotbarUI(Dictionary<int, InventoryEntry> hotbarState)
        {
            hotbarUI.ResetAllItems(); // resets all data and items
            foreach (var item in hotbarState)
            {
                hotbarUI.UpdateData(item.Key, item.Value.item.ItemImage, item.Value.quantity);
            }
        }

        private void HandleHotbarDescriptionRequest(int itemIndex) // handles the description when selecting hotbar item
        {
            InventoryEntry hotbarItem = hotbarData.GetItemAt(itemIndex);
            if (hotbarItem.IsEmpty)
            {
                inventoryUI.ResetSelection(); // Reset the shared description panel
                hotbarUI.ResetSelection(); // Reset hotbar selection too
                return;
            }
            ItemSO item = hotbarItem.item;
            string description = PrepareDescription(hotbarItem);
            inventoryUI.UpdateDescription(itemIndex, item.ItemImage, item.Name, description); // Use inventory UI's description panel
            inventoryUI.ResetSelection(); // Clear inventory border
            // Hotbar will keep its selection border
        }

        private void HandleHotbarSwapItems(int itemIndex1, int itemIndex2) // handles swapping items within hotbar
        {
            hotbarData.SwapItems(itemIndex1, itemIndex2);
        }

        private void HandleHotbarDragging(int itemIndex) // helps with getting information while performing drag in hotbar
        {
            InventoryEntry hotbarItem = hotbarData.GetItemAt(itemIndex);
            if (hotbarItem.IsEmpty)
                return;
            
            currentDragSource = ItemSource.Hotbar;
            currentDragSourceIndex = itemIndex;
            
            hotbarUI.CreateDraggedItem(hotbarItem.item.ItemImage, hotbarItem.quantity);
        }

        private void HandleHotbarItemActionRequest(int itemIndex) // this will let us show options when right clicking hotbar items
        {
            InventoryEntry hotbarItem = hotbarData.GetItemAt(itemIndex);
            if (hotbarItem.IsEmpty)
                return;

            // lets us interact with the item
            IItemAction itemAction = hotbarItem.item as IItemAction;
            if (itemAction != null)
            {
                hotbarUI.ShowItemAction(itemIndex);
                hotbarUI.AddAction(itemAction.ActionName, () => PerformHotbarAction(itemIndex));
            }

            // removes the item from hotbar
            IDestroyableItem destroyableItem = hotbarItem.item as IDestroyableItem;
            if (destroyableItem != null)
            {
                hotbarUI.AddAction("Remove", () => RemoveHotbarItem(itemIndex, hotbarItem.quantity));
            }
        }

        private void RemoveHotbarItem(int itemIndex, int quantity)
        {
            hotbarData.RemoveItem(itemIndex, quantity); // removes item from the hotbar data
            hotbarUI.ResetSelection(); // then deselects the item
            audioSource.PlayOneShot(dropItemClip);
        }

        public void PerformHotbarAction(int itemIndex)
        {
            InventoryEntry hotbarItem = hotbarData.GetItemAt(itemIndex);
            if (hotbarItem.IsEmpty)
                return;

            // destroys the item when removing or equipping
            IDestroyableItem destroyableItem = hotbarItem.item as IDestroyableItem;
            if (destroyableItem != null)
            {
                hotbarData.RemoveItem(itemIndex, 1);
            }

            // lets us interact with the item
            IItemAction itemAction = hotbarItem.item as IItemAction;
            if (itemAction != null)
            {
                itemAction.PerformAction(gameObject, hotbarItem.itemState);
                if (itemAction.actionSFX != null)
                {
                    AudioSource.PlayClipAtPoint(itemAction.actionSFX, transform.position);
                }
                if (hotbarData.GetItemAt(itemIndex).IsEmpty)
                    hotbarUI.ResetSelection();
            }
        }

        // ========== CROSS-INVENTORY TRANSFER HANDLERS ==========

        private void HandleInventoryItemDropped(int targetIndex)
        {
            // Item dropped on inventory slot
            if (currentDragSource == ItemSource.Hotbar && currentDragSourceIndex >= 0)
            {
                // Transfer from hotbar to inventory
                TransferItemFromHotbarToInventory(currentDragSourceIndex, targetIndex);
            }
            // Reset drag state
            currentDragSourceIndex = -1;
            inventoryUI.ResetDraggedItem();
            hotbarUI.ResetDraggedItem();
        }

        private void HandleHotbarItemDropped(int targetIndex)
        {
            // Item dropped on hotbar slot
            if (currentDragSource == ItemSource.Inventory && currentDragSourceIndex >= 0)
            {
                // Transfer from inventory to hotbar
                TransferItemFromInventoryToHotbar(currentDragSourceIndex, targetIndex);
            }
            // Reset drag state
            currentDragSourceIndex = -1;
            inventoryUI.ResetDraggedItem();
            hotbarUI.ResetDraggedItem();
        }

        private void TransferItemFromInventoryToHotbar(int inventoryIndex, int hotbarIndex)
        {
            InventoryEntry itemToTransfer = inventoryData.GetItemAt(inventoryIndex);
            if (itemToTransfer.IsEmpty)
                return;

            InventoryEntry hotbarSlot = hotbarData.GetItemAt(hotbarIndex);

            // If hotbar slot is empty, move the item
            if (hotbarSlot.IsEmpty)
            {
                hotbarData.AddItemAtSlot(hotbarIndex, itemToTransfer.item, itemToTransfer.quantity, itemToTransfer.itemState);
                inventoryData.RemoveItem(inventoryIndex, itemToTransfer.quantity);
            }
            // If both items are the same and stackable, try to stack
            else if (hotbarSlot.item == itemToTransfer.item && itemToTransfer.item.IsStackable)
            {
                int spaceInHotbar = hotbarSlot.item.MaxStackSize - hotbarSlot.quantity;
                int amountToMove = Mathf.Min(itemToTransfer.quantity, spaceInHotbar);

                if (amountToMove > 0)
                {
                    hotbarData.AddItemAtSlot(hotbarIndex, itemToTransfer.item, hotbarSlot.quantity + amountToMove, hotbarSlot.itemState);
                    inventoryData.RemoveItem(inventoryIndex, amountToMove);
                }
            }
            // Otherwise swap the items
            else
            {
                inventoryData.SwapItems(inventoryIndex, inventoryIndex); // temp to force refresh
                hotbarData.SwapItems(hotbarIndex, hotbarIndex); // temp to force refresh
                
                // Swap items between systems
                InventoryEntry temp = itemToTransfer;
                inventoryData.RemoveItem(inventoryIndex, itemToTransfer.quantity);
                hotbarData.RemoveItem(hotbarIndex, hotbarSlot.quantity);
                
                inventoryData.AddItemAtSlot(inventoryIndex, hotbarSlot.item, hotbarSlot.quantity, hotbarSlot.itemState);
                hotbarData.AddItemAtSlot(hotbarIndex, temp.item, temp.quantity, temp.itemState);
            }
        }

        private void TransferItemFromHotbarToInventory(int hotbarIndex, int inventoryIndex)
        {
            InventoryEntry itemToTransfer = hotbarData.GetItemAt(hotbarIndex);
            if (itemToTransfer.IsEmpty)
                return;

            InventoryEntry inventorySlot = inventoryData.GetItemAt(inventoryIndex);

            // If inventory slot is empty, move the item
            if (inventorySlot.IsEmpty)
            {
                inventoryData.AddItemAtSlot(inventoryIndex, itemToTransfer.item, itemToTransfer.quantity, itemToTransfer.itemState);
                hotbarData.RemoveItem(hotbarIndex, itemToTransfer.quantity);
            }
            // If both items are the same and stackable, try to stack
            else if (inventorySlot.item == itemToTransfer.item && itemToTransfer.item.IsStackable)
            {
                int spaceInInventory = inventorySlot.item.MaxStackSize - inventorySlot.quantity;
                int amountToMove = Mathf.Min(itemToTransfer.quantity, spaceInInventory);

                if (amountToMove > 0)
                {
                    inventoryData.AddItemAtSlot(inventoryIndex, itemToTransfer.item, inventorySlot.quantity + amountToMove, inventorySlot.itemState);
                    hotbarData.RemoveItem(hotbarIndex, amountToMove);
                }
            }
            // Otherwise swap the items
            else
            {
                inventoryData.SwapItems(inventoryIndex, inventoryIndex); // temp to force refresh
                hotbarData.SwapItems(hotbarIndex, hotbarIndex); // temp to force refresh
                
                // Swap items between systems
                InventoryEntry temp = itemToTransfer;
                hotbarData.RemoveItem(hotbarIndex, itemToTransfer.quantity);
                inventoryData.RemoveItem(inventoryIndex, inventorySlot.quantity);
                
                hotbarData.AddItemAtSlot(hotbarIndex, inventorySlot.item, inventorySlot.quantity, inventorySlot.itemState);
                inventoryData.AddItemAtSlot(inventoryIndex, temp.item, temp.quantity, temp.itemState);
            }
        }

        public void ShowInventory()
        {
            if (inventoryUI != null)
            {
                inventoryUI.Show();
            }
        }

        public void HideInventory()
        {
            if (inventoryUI != null)
            {
                inventoryUI.Hide();
            }
        }
    }
}
