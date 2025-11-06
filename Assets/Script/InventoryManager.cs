using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public GameObject InventoryMenu;
    private Actions controls;
    private bool menuActive;

    void Awake()
    {
        controls = new Actions();
        controls.Player.Inventory.performed += ctx => ToggleInventory();
    }

    void OnEnable()
    {
        controls.Enable();
    }

    void OnDisable()
    {
        controls.Disable();
    }

    private void ToggleInventory()
    {
        menuActive = !menuActive;
        InventoryMenu.SetActive(menuActive);
    }

    public void AddItem(string itemName, int quantity, Sprite itemSprite)
    {
        Debug.Log("itemName = " + itemName + "quantity = " + quantity + "item sprite = " + itemSprite);
    }
}
