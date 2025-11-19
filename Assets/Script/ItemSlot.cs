using System;
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
    [SerializeField] public Sprite emptySprite;

    [SerializeField] private int maxNumberOfItems;

    [SerializeField] private TMP_Text quantityText;

    [SerializeField] private Image itemImage;

    public Image itemDescriptionImage;
    public TMP_Text ItemDescriptionNameText;
    public TMP_Text ItemDescriptionText;

    [SerializeField] public GameObject selectedShader;
    public bool thisItemSelected;

    private InventoryManager inventoryManager;

    // for how to drag the item around
    public event Action<InventoryManager> OnItemClicked, OnItemDropped, OnItemBeginDrag, OnItemEndDrag, OnRightMouseBtnClick;

    private bool isEmpty = true;

    void Start()
    {
        // Find InventoryManager anywhere in the scene, even if nested under Player
        inventoryManager = FindObjectOfType<InventoryManager>();
        if (inventoryManager == null)
        {
            Debug.LogError("InventoryManager not found! Make sure it exists in the scene.");
        }
        UpdateQuantityDisplay();
    }

    private void UpdateQuantityDisplay()
    {
        if (quantity > 0)
        {
            quantityText.text = quantity.ToString();
            quantityText.enabled = true;
        }
        else
        {
            quantityText.enabled = false;
        }
    }

public int AddItem(string itemName, int quantity, Sprite itemSprite, string itemDescription)
{
    if (isFull)
    {
        return quantity;
    }

    this.itemName = itemName;
    this.itemSprite = itemSprite;
    itemImage.sprite = itemSprite;
    this.itemDescription = itemDescription;

    this.quantity += quantity;
    
    if (this.quantity >= maxNumberOfItems)
    {
        isFull = true;
        int extraItems = this.quantity - maxNumberOfItems;
        this.quantity = maxNumberOfItems;
        quantityText.text = maxNumberOfItems.ToString();
        quantityText.enabled = true;
        return extraItems;
    }
    else
    {
        isFull = false;
    }
    
    // Update quantity display
    UpdateQuantityDisplay();
    return 0;
}


    public void AddQuantity(int amount) // adds item on top of already existing item
    {
        quantity += amount;
        quantityText.text = quantity.ToString();
        quantityText.enabled = true;
        
        // If quantity reaches 0 or below, clear the slot
        if (quantity <= 0)
        {
            ClearSlot();
        }
    }

    public void ClearSlot() // clears the slot
    {
        itemName = "";
        quantity = 0;
        itemSprite = null;
        isFull = isFull = (this.quantity >= maxNumberOfItems);
        itemImage.sprite = emptySprite;
        quantityText.text = "";
        quantityText.enabled = false;
        EmptySlot(); // also will empty the slot just in case
    }

    void Update()
    {

    }

    public void OnPointerClick(PointerEventData eventData) // maps clicks to each function below
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

    public void OnBeginDrag() {
        if (isEmpty) {
            return;
        }
        OnItemBeginDrag?.Invoke(inventoryManager);
    }

    public void OnDrop() {
        OnItemDropped?.Invoke(inventoryManager);
    }

    public void OnEndDrag() {
        OnItemEndDrag?.Invoke(inventoryManager);
    }

    public void OnLeftClick()
    {
        if (thisItemSelected)
        {
            bool usable = inventoryManager.UseItem(itemName);
            if (usable == true)
            {
                this.quantity -= 1;
                isFull = (this.quantity >= maxNumberOfItems);
                ;
                quantityText.text = this.quantity.ToString();
                if (this.quantity <= 0)
                {
                    EmptySlot();
                }
            }
        }
        else
        {
            // Only show item details if slot has an item
            if (string.IsNullOrEmpty(itemName) || quantity <= 0)
                return;
                
            inventoryManager.DeselectAllSlots();
            selectedShader.SetActive(true);
            thisItemSelected = true;
            ItemDescriptionNameText.text = itemName;
            ItemDescriptionText.text = itemDescription;
            itemDescriptionImage.sprite = itemSprite;
            if (itemDescriptionImage.sprite == null)
            {
                itemDescriptionImage.sprite = emptySprite;
            }
            
            // Notify listeners (like CraftingUIController) that item was clicked
            OnItemClicked?.Invoke(inventoryManager);
        }

    }

    public void OnRightClick()
    {

        if (string.IsNullOrEmpty(itemName) || quantity <= 0)
            return;

        GameObject itemToDrop = new GameObject(itemName);
        Item newItem = itemToDrop.AddComponent<Item>();
        newItem.quantity = 1;
        newItem.itemName = itemName;
        newItem.sprite = itemSprite;
        newItem.itemDescription = itemDescription;

        // the item can now drop back on the floor
        SpriteRenderer sr = itemToDrop.AddComponent<SpriteRenderer>();
        sr.sprite = itemSprite;
        sr.sortingOrder = 5;
        sr.sortingLayerName = "Player";

        // gives item a collider 
        BoxCollider2D collider = itemToDrop.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;

        // location of the dropped item, can modify the left and right, with 0 being center.
        Vector2 randomOffset = UnityEngine.Random.insideUnitCircle.normalized * .5f; // 1 unit distance in a random direction
        itemToDrop.transform.position = GameObject.FindWithTag("Player").transform.position + new Vector3(randomOffset.x, randomOffset.y, 0);
        //item size 
        itemToDrop.transform.localScale = new Vector3(0.25f, 0.25f, 1f);

        this.quantity -= 1;
        isFull = (this.quantity >= maxNumberOfItems);
        quantityText.text = this.quantity.ToString();
        if (this.quantity <= 0)
        {
            EmptySlot();
        }

    }

    private void EmptySlot() // for empty slots in inventory
    {
        quantityText.enabled = false;
        itemImage.sprite = emptySprite;

        ItemDescriptionNameText.text = "";
        ItemDescriptionText.text = "";
        itemDescriptionImage.sprite = emptySprite;


    }
}
