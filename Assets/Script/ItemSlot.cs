using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    public string itemName;
    public int quantity;
    public Sprite itemSprite;
    public bool isFull;


    [SerializeField] private TMP_Text quantityText;

    [SerializeField] private Image itemImage;

    public GameObject selectedShader;
    public bool thisItemSelected;

    void Start()
    {
        quantityText.enabled = false;
    }

    public void AddItem(string itemName, int quantity, Sprite itemSprite)
    {
        this.itemName = itemName;
        this.quantity = quantity;
        this.itemSprite = itemSprite;
        isFull = true;

        quantityText.text = quantity.ToString(); // the string value of the int is assigned to the TMPro which is in text form. confusing i know
        quantityText.enabled = true;
        itemImage.sprite = itemSprite;
    }

    public void AddQuantity(int amount)
    {
        quantity += amount;
        quantityText.text = quantity.ToString();
        quantityText.enabled = true;
    }

    public void ClearSlot()
    {
        itemName = "";
        quantity = 0;
        itemSprite = null;
        isFull = false;
        itemImage.sprite = null;
        quantityText.text = "";
        quantityText.enabled = false;
    }

    void Update()
    {

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnLeftClick();
        }
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            OnRightClick();
        }
    }

    public void OnLeftClick()
    {
        selectedShader.SetActive(true);
        thisItemSelected = true;
    }
    
        public void OnRightClick()
    {
        
    }
}
