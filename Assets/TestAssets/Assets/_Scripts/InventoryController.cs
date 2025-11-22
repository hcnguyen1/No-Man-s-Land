using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{

    [SerializeField]
    private InventoryPage inventoryUI;

    [SerializeField]
    private InventorySO inventoryData;

    private void Start()
    {
        PrepareUI();
        //inventoryData.Initialize();
    }

    private void PrepareUI()
    {
        inventoryUI.IntializeInventoryUI(inventoryData.Size);
        this.inventoryUI.OnDescriptionRequested += HandleDescriptionRequest;
        this.inventoryUI.OnSwapItems += HandleSwapItems;
        this.inventoryUI.OnStartDragging += HandleDragging;
        this.inventoryUI.OnItemActionRequested += HandleItemActionRequest;
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (inventoryUI.isActiveAndEnabled == false)
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

    private void HandleDescriptionRequest(int itemIndex)
    {
        InventoryItemm inventoryItem = inventoryData.GetItemAt(itemIndex);
        if (inventoryItem.IsEmpty)
        {
            inventoryUI.ResetSelection();
            return;
        }
        ItemSO item = inventoryItem.item;
        inventoryUI.UpdateDescription(itemIndex, item.ItemImage, item.Name, item.Description);
    }

    private void HandleSwapItems(int itemIndex1, int itemIndex2)
    {

    }

    private void HandleDragging(int itemIndex)
    {

    }

    private void HandleItemActionRequest(int itemIndex)
    {

    }
}
