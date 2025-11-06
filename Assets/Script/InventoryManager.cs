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
    }

    public void AddItem(string itemName, int quantity, Sprite itemSprite, string itemDescription)
    {

        for (int i = 0; i < itemSlot.Length; i++)
        {
            if (itemSlot[i] == null) continue;
            if (itemSlot[i].isFull && itemSlot[i].itemName == itemName)
            {
                itemSlot[i].AddQuantity(quantity);
                return;
            }
        }

        for (int i = 0; i < itemSlot.Length; i++)
        {
            if (itemSlot[i].isFull == false)
            {
                itemSlot[i].AddItem(itemName, quantity, itemSprite, itemDescription);
                return;
            }
        }
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
