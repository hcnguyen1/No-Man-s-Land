using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Inventory.Model;

namespace Inventory.UI
{
    public class HotbarUI : MonoBehaviour
    {
        [SerializeField]
        private InventoryItem itemPrefab;

        [SerializeField]
        private RectTransform contentPanel;

        [SerializeField]
        private ItemActionPanel actionPanel;

        [SerializeField]
        private InventoryDescription itemDescription;

        [SerializeField]
        private MouseFollower mouseFollower;

        List<InventoryItem> listOfUIItems = new List<InventoryItem>();

        private int currentlyDraggedItemIndex = -1;

        public event Action<int> OnDescriptionRequested, OnItemActionRequested, OnStartDragging;
        public event Action<int, int> OnSwapItems; // passes 2 indexes of int type.
        public event Action<int> OnItemDroppedOn; // for cross-inventory transfers

        private void Awake()
        {
            // Hotbar should be visible by default (unlike inventory which starts hidden)
        }

        public void IntializeHotbarUI(int hotbarsize) // this will create hotbar as well as add items and hook it to the contentPanel
        {
            for (int i = 0; i < hotbarsize; i++)
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

        private void HandleItemSelection(InventoryItem obj)
        {
            int index = listOfUIItems.IndexOf(obj);
            if (index >= 0)
            {
                OnDescriptionRequested?.Invoke(index);
            }
        }

        private void HandleBeginDrag(InventoryItem obj)
        {
            currentlyDraggedItemIndex = listOfUIItems.IndexOf(obj);
            OnStartDragging?.Invoke(currentlyDraggedItemIndex);
        }

        private void HandleSwap(InventoryItem obj)
        {
            int targetItemIndex = listOfUIItems.IndexOf(obj);
            // For cross-inventory transfers, always invoke OnItemDroppedOn - the controller will handle the source
            OnItemDroppedOn?.Invoke(targetItemIndex); // for cross-inventory transfers
            currentlyDraggedItemIndex = -1;
        }

        private void HandleEndDrag(InventoryItem obj)
        {
            currentlyDraggedItemIndex = -1;
        }

        private void HandleShowItemActions(InventoryItem obj)
        {
            int index = listOfUIItems.IndexOf(obj);
            if (index >= 0)
            {
                OnItemActionRequested?.Invoke(index);
            }
        }

        public void UpdateData(int itemIndex, Sprite itemImage, int itemQuantity)
        {
            if (itemIndex >= 0 && itemIndex < listOfUIItems.Count)
            {
                listOfUIItems[itemIndex].SetData(itemImage, itemQuantity);
            }
        }

        public void ResetAllItems()
        {
            foreach (InventoryItem item in listOfUIItems)
            {
                item.ResetData();
                item.Deselect();
            }
        }

        public void UpdateDescription(int itemIndex, Sprite itemImage, string name, string description)
        {
            if (itemDescription != null)
                itemDescription.SetDescription(itemImage, name, description);
            DeselectAllItems();
            listOfUIItems[itemIndex].Select();
        }

        public void CreateDraggedItem(Sprite sprite, int quantity)
        {
            mouseFollower.Toggle(true);
            mouseFollower.SetData(sprite, quantity);
        }

        public void ResetDraggedItem()
        {
            mouseFollower.Toggle(false);
            currentlyDraggedItemIndex = -1;
        }

        public void ResetSelection()
        {
            if (itemDescription != null)
                itemDescription.ResetDescription();
            DeselectAllItems();
        }

        private void DeselectAllItems()
        {
            foreach (InventoryItem item in listOfUIItems)
            {
                item.Deselect();
            }
            actionPanel.Toggle(false);
        }

        public void AddAction(string actionName, Action performAction)
        {
            actionPanel.AddButton(actionName, performAction);
        }

        public void ShowItemAction(int itemIndex)
        {
            actionPanel.Toggle(true);
            actionPanel.transform.position = listOfUIItems[itemIndex].transform.position;
        }

        public void Show()
        {
            gameObject.SetActive(true);
            ResetSelection();
        }

        public void Hide()
        {
            actionPanel.Toggle(false);
            gameObject.SetActive(false);
            ResetDraggedItem();
        }
    }
}
