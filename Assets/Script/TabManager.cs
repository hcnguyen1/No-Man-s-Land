using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class TabManager : MonoBehaviour
{
    public GameObject[] Tabs;
    public Image[] TabButtons;
    public Sprite InactiveTabBG, ActiveTabBG;
    public Vector2 InactiveTabButtonSize, ActiveTabButtonSize;

    [Header("Menu References")]
    public GameObject characterMenu;
    public GameObject inventoryMenu;
    public GameObject craftingMenu;
    public Image itemDescriptionImage; // The item image to hide when crafting
    public Image itemDescriptionBackground; // The background of the description panel
    public TMP_Text itemDescriptionNameText; // The item name text
    public TMP_Text itemDescriptionText; // The item description text
    
    private InventoryManager inventoryManager;
    private Player player;

    void Start()
    {
        inventoryManager = FindObjectOfType<InventoryManager>();
        player = FindObjectOfType<Player>();
    }

    void Update()
    {
        // Close crafting menu if player walks away from bench
        if (craftingMenu != null && craftingMenu.activeSelf && player != null)
        {
            if (!player.canOpenCraftingMenu)
            {
                craftingMenu.SetActive(false);
                // Re-enable description components when crafting closes
                if (itemDescriptionImage != null)
                    itemDescriptionImage.enabled = true;
                if (itemDescriptionBackground != null)
                    itemDescriptionBackground.enabled = true;
                if (itemDescriptionNameText != null)
                    itemDescriptionNameText.enabled = true;
                if (itemDescriptionText != null)
                    itemDescriptionText.enabled = true;
            }
        }

        // Keyboard shortcuts
        if (Input.GetKeyDown(KeyCode.C))
            OpenCharacterMenu();
        
        if (Input.GetKeyDown(KeyCode.K))
            OpenCraftingMenu();
    }

    public void OpenCharacterMenu()
    {
        if (characterMenu != null)
        {
            CloseAllMenus();
            characterMenu.SetActive(true);
        }
    }

    public void OpenInventoryMenu()
    {
        // Use InventoryManager's toggle so E button and tab button stay in sync
        if (inventoryManager != null)
        {
            // Only toggle if inventory is currently closed
            if (inventoryMenu != null && !inventoryMenu.activeSelf)
            {
                // Close other menus first
                if (characterMenu != null) characterMenu.SetActive(false);
                if (craftingMenu != null) craftingMenu.SetActive(false);
                
                // Let InventoryManager handle opening
                inventoryManager.SendMessage("ToggleInventory");
            }
        }
    }

    public void OpenCraftingMenu()
    {
        // Only allow opening crafting menu if near bench
        if (craftingMenu != null && player != null && player.canOpenCraftingMenu)
        {
            // Close character menu only
            if (characterMenu != null) 
                characterMenu.SetActive(false);
            
            // Ensure inventory menu is open using InventoryManager's toggle
            if (inventoryMenu != null && !inventoryMenu.activeSelf && inventoryManager != null)
            {
                // Call InventoryManager's toggle to properly sync state
                inventoryManager.SendMessage("ToggleInventory");
            }
            
            // Hide description components (but keep GameObject active for layout)
            if (itemDescriptionImage != null)
                itemDescriptionImage.enabled = false;
            if (itemDescriptionBackground != null)
                itemDescriptionBackground.enabled = false;
            if (itemDescriptionNameText != null)
                itemDescriptionNameText.enabled = false;
            if (itemDescriptionText != null)
                itemDescriptionText.enabled = false;
                
            craftingMenu.SetActive(true);
        }
    }

    private void CloseAllMenus()
    {
        if (characterMenu != null) characterMenu.SetActive(false);
        if (inventoryMenu != null) inventoryMenu.SetActive(false);
        if (craftingMenu != null) craftingMenu.SetActive(false);
    }

public void SwitchToTab(int TabID)
{
    foreach (GameObject go in Tabs)
    {
        go.SetActive(false);
    }
    Tabs[TabID].SetActive(true);

    foreach (Image im in TabButtons)
    {
        im.sprite = InactiveTabBG;
        im.rectTransform.sizeDelta = InactiveTabButtonSize;
    }
    TabButtons[TabID].sprite = ActiveTabBG;
    TabButtons[TabID].rectTransform.sizeDelta = ActiveTabButtonSize;
}

}
