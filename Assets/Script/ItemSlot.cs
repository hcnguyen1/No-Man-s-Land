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
    public string itemDescription;

    [SerializeField] private TMP_Text quantityText;

    [SerializeField] private Image itemImage;

    public Image itemDescriptionImage;
    public TMP_Text ItemDescriptionNameText;
    public TMP_Text ItemDescriptionText;

    [SerializeField] public GameObject selectedShader;
    public bool thisItemSelected;

    private InventoryManager inventoryManager;

    void Start()
    {
        quantityText.enabled = false;
        inventoryManager = GameObject.Find("UI").GetComponent<InventoryManager>();
    }

    public void AddItem(string itemName, int quantity, Sprite itemSprite, string itemDescription)
    {
        this.itemName = itemName;
        this.quantity = quantity;
        this.itemSprite = itemSprite;
        this.itemDescription = itemDescription;
        isFull = true;

        quantityText.text = quantity.ToString(); // the string value of the int is assigned to the TMPro which is in text form. confusing i know
        quantityText.enabled = true;
        itemImage.sprite = itemSprite;
        itemImage.enabled = true;
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
        itemImage.enabled = false;
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
        inventoryManager.DeselectAllSlots();
        selectedShader.SetActive(true);
        thisItemSelected = true;
        ItemDescriptionNameText.text = itemName;
        ItemDescriptionText.text = itemDescription;
        itemDescriptionImage.sprite = itemSprite;
    }

    public void OnRightClick()
    {

    }
}
