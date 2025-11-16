using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public GameObject InventoryMenu;
    private Actions controls;
    private bool menuActive;
    [SerializeField] public ItemSlot[] itemSlot; // the array of item slots now it becomes a inventory.

    [SerializeField] public ItemSO[] itemSOs;

    [Header("Testing - Starting Items")]
    [Tooltip("Items that will be added to inventory on start (for testing)")]
    public ItemSO[] startingItems;

    void Start()
    {
        // Add starting items for testing
        if (startingItems != null && startingItems.Length > 0)
        {
            foreach (ItemSO item in startingItems)
            {
                if (item != null)
                {
                    AddItem(item.itemName, 1, item.icon, item.itemDescription);
                }
            }
        }
    }

    void Awake()
    {
        controls = new Actions();
        controls.Player.Inventory.performed += ctx => ToggleInventory();
    }

    void OnEnable()
    {
        if (controls != null)
            controls.Enable();
    }

    void OnDisable()
    {
        if (controls != null)
            controls.Disable();
    }


    private void ToggleInventory()
    {
        menuActive = !menuActive;
        InventoryMenu.SetActive(menuActive);
        
        // Close character menu when opening inventory
        if (menuActive)
        {
            TabManager tabManager = FindObjectOfType<TabManager>();
            if (tabManager != null && tabManager.characterMenu != null)
            {
                tabManager.characterMenu.SetActive(false);
            }
        }
        else
        {
            // When closing inventory, also close crafting menu
            TabManager tabManager = FindObjectOfType<TabManager>();
            if (tabManager != null && tabManager.craftingMenu != null)
            {
                tabManager.craftingMenu.SetActive(false);
                // Re-enable description components
                if (tabManager.itemDescriptionImage != null)
                    tabManager.itemDescriptionImage.enabled = true;
                if (tabManager.itemDescriptionBackground != null)
                    tabManager.itemDescriptionBackground.enabled = true;
                if (tabManager.itemDescriptionNameText != null)
                    tabManager.itemDescriptionNameText.enabled = true;
                if (tabManager.itemDescriptionText != null)
                    tabManager.itemDescriptionText.enabled = true;
            }
        }
    }

    public void OpenInventory()
    {
        if (!InventoryMenu.activeSelf)
        {
            menuActive = true;
            InventoryMenu.SetActive(true);
            
            // Close character menu when opening inventory
            TabManager tabManager = FindObjectOfType<TabManager>();
            if (tabManager != null && tabManager.characterMenu != null)
            {
                tabManager.characterMenu.SetActive(false);
            }
        }
    }

    public bool UseItem(string itemName)
    {
        for (int i = 0; i < itemSOs.Length; i++)
        {
            if (itemSOs[i].itemName == itemName)
            {
                bool usable = itemSOs[i].UseItem();
                return usable;
            }
        }
        return false;
    }

    public int AddItem(string itemName, int quantity, Sprite itemSprite, string itemDescription)
    {
        for (int i = 0; i < itemSlot.Length; i++)
        {
            if ((itemSlot[i].isFull == false && itemSlot[i].itemName == itemName) || itemSlot[i].quantity == 0)
            {
                int leftOverItems = itemSlot[i].AddItem(itemName, quantity, itemSprite, itemDescription);
                if (leftOverItems > 0) // you want to let the items flow to the next slot rather than not letting your character pick up anymore.
                {
                    leftOverItems = AddItem(itemName, leftOverItems, itemSprite, itemDescription);
                }
                return leftOverItems;
            }
        }
        return quantity;
    }

    public void DeselectAllSlots()
    {
        for (int i = 0; i < itemSlot.Length; i++)
        {
            if (itemSlot[i] == null) continue;
            if (itemSlot[i].selectedShader != null)
                itemSlot[i].selectedShader.SetActive(false);
            itemSlot[i].thisItemSelected = false;
        }
    }
}
