using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;

namespace Inventory.UI
{
    public class InventoryItem : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IEndDragHandler, IDropHandler, IDragHandler
    {

        [SerializeField]
        private Image itemImage; // item image 

        [SerializeField]
        private TMP_Text quantityText; // the text of the item 

        [SerializeField]
        private Image borderImage; // the border to select 

        public event Action<InventoryItem> OnItemClicked, OnItemDroppedOn, OnItemBeginDrag, OnItemEndDrag, OnRightMouseBtnClick; // interactions with items using mouse
                                                                                                                                 // OnItemClick interacts with item, onItemDroppedOn will allow is to set the destination, OnItemBeginDrag allows start of movement of item, 
                                                                                                                                 // end is the ending of that, right mouse gives us 2 options.

        private bool isEmpty = true; // if the slot is empty, then cannot use Actions above. 


        public void Awake()
        {
            ResetData(); // on awake, reset data
            Deselect(); // on awake, deselect the slot
        }

        public void ResetData() // clears the inventory item image.
        {
            this.itemImage.gameObject.SetActive(false); // inactive
            isEmpty = true; // now the slot will be empty.
        }

        public void Deselect() // deselects the item so the border will be disabled. 
        {
            borderImage.enabled = false;
        }

        public void SetData(Sprite sprite, int quantity) // sets data to the inventory item slot
        {
            this.itemImage.gameObject.SetActive(true); // active again
            this.itemImage.sprite = sprite; // image becomes whatever is currently there.
            this.quantityText.text = quantity + ""; // instead of nothing it will be whatever quantity.
            isEmpty = false; // slot is now occupied. 
        }

        public void Select() // upon selecting the item, the border image will be highlighting the item 
        {
            borderImage.enabled = true;
        }

        public void OnPointerClick(PointerEventData pointerData) // depending on what mousebutton is used
        {
            if (pointerData.button == PointerEventData.InputButton.Right) // if clicked with right click
            {
                OnRightMouseBtnClick?.Invoke(this);
            }
            else // else on left click 
            {
                OnItemClicked?.Invoke(this);
            }
        }

        public void OnBeginDrag(PointerEventData eventData) // if item slot isnt empty, begin dragging the item 
        {
            if (isEmpty)
                return;
            OnItemBeginDrag?.Invoke(this);
        }

        public void OnDrop(PointerEventData eventData) // when you drop the item from mouse holding it
        {
            OnItemDroppedOn?.Invoke(this);
        }


        public void OnEndDrag(PointerEventData eventData) // ending of the dragging of the item
        {
            OnItemEndDrag?.Invoke(this);
        }

        public void OnDrag(PointerEventData eventData)
        {

        }
    }

}
