using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Inventory.UI
{
    public class InventoryPage : MonoBehaviour
    {

        [SerializeField]
        private InventoryItem itemPrefab;

        [SerializeField]
        private RectTransform contentPanel;

        [SerializeField]
        private InventoryDescription itemDescription;


        [SerializeField]
        private MouseFollower mouseFollower;


        List<InventoryItem> listOfUIItems = new List<InventoryItem>();

        private int currentlyDraggedItemIndex = -1;

        public event Action<int> OnDescriptionRequested, OnItemActionRequested, OnStartDragging;
        // on description will add border, request will give right button options, dragging will give quantity 

        public event Action<int, int> OnSwapItems; // same as dragging but passes 2 indexes of int type. 


        private void Awake()
        {
            Hide(); // hides inventory page
            mouseFollower.Toggle(false);
            itemDescription.ResetDescription();
        }

        public void IntializeInventoryUI(int inventorysize) // this will create inventory as well as add items and hook it to the contentPanel
        {
            for (int i = 0; i < inventorysize; i++)
            {
                InventoryItem uiItem = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
                uiItem.transform.SetParent(contentPanel);
                listOfUIItems.Add(uiItem);
                uiItem.OnItemClicked += HandleItemSelection;
                uiItem.OnItemBeginDrag += HandleBeginDrag;
                uiItem.OnItemDroppedOn += HandleSwap;
                uiItem.OnItemEndDrag += HandleEndDrag;
                uiItem.OnRightMouseBtnClick += HandleShowItemActions;
            }
        }

        internal void ResetAllItems() // called from inventoryController where you reset data and unselect. 
        {
            foreach (var item in listOfUIItems)
            {
                item.ResetData();
                item.Deselect();
            }
        }

        internal void UpdateDescription(int itemIndex, Sprite itemImage, string name, string description)
        { // method title is obvious
            itemDescription.SetDescription(itemImage, name, description);
            DeselectAllItems();
            listOfUIItems[itemIndex].Select();
        }

        public void UpdateData(int itemIndex, Sprite itemImage, int itemQuantity)
        {// takes the index of the list of items and updates the image and quantity when picked up, dropped or anything else
            if (listOfUIItems.Count > itemIndex)
            {
                listOfUIItems[itemIndex].SetData(itemImage, itemQuantity);
            }
        }

        private void HandleItemSelection(InventoryItem InventoryItemUI) // will tell us what item is being held
        {
            int index = listOfUIItems.IndexOf(InventoryItemUI);
            if (index == -1)
            {
                return;
            }
            OnDescriptionRequested?.Invoke(index); // if we select an item and we have it in the list, we can pass and update inventory controller.
        }

        private void HandleSwap(InventoryItem InventoryItemUI) // will swap if theres a legitimate item in the slot basically. it will check the item mid swap
        {
            int index = listOfUIItems.IndexOf(InventoryItemUI);

            if (index == -1)
            {
                return;
            }
            OnSwapItems?.Invoke(currentlyDraggedItemIndex, index);
            HandleItemSelection(InventoryItemUI);
        }

        private void HandleBeginDrag(InventoryItem InventoryItemUI)
        {
            int index = listOfUIItems.IndexOf(InventoryItemUI);

            if (index == -1) // fills in the data if it returns something outside of the inventory, like something that doesn't belong.
                return;
            currentlyDraggedItemIndex = index; // else you can start to drag the item 
            HandleItemSelection(InventoryItemUI);
            OnStartDragging?.Invoke(index); // should we start dragging item ternary
        }

        public void CreateDraggedItem(Sprite sprite, int quantity)
        { // helps mousefollower.cs by setting the data and adjusting the ghost view
            mouseFollower.Toggle(true);
            mouseFollower.SetData(sprite, quantity);
        }



        private void HandleEndDrag(InventoryItem InventoryItemUI)
        { // after dragging we reset the items position
            ResetDraggedItem();
        }

        private void HandleShowItemActions(InventoryItem InventoryItemUI)
        {

        }


        public void Show() // this lets us show the inventory page 
        {
            gameObject.SetActive(true);
            ResetSelection(); // since we reset selecting the item, we want to reset description as well.
        }

        public void Hide() // this will let us hide the inventory page
        {
            gameObject.SetActive(false);
            ResetDraggedItem();
        }

        public void ResetDraggedItem() // in case you dont want to drag it anywhere it can go back to its original slot. 
        {
            mouseFollower.Toggle(false);
            currentlyDraggedItemIndex = -1;
        }

        public void ResetSelection()
        {
            itemDescription.ResetDescription(); // if theres data, we can fill it using this. 
            DeselectAllItems();
        }

        private void DeselectAllItems()
        { // clears the border image when you don't pick an item
            foreach (InventoryItem item in listOfUIItems)
            {
                item.Deselect();
            }
        }
    }

}
