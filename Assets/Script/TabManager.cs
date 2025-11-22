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
    //public GameObject characterMenu;
    //public GameObject inventoryMenu;
    //public GameObject craftingMenu;
    public Image itemDescriptionImage; // The item image to hide when crafting
    public Image itemDescriptionBackground; // The background of the description panel
    public TMP_Text itemDescriptionNameText; // The item name text
    public TMP_Text itemDescriptionText; // The item description text

    //private InventoryManager inventoryManager;
    private Player player;

    void Start()
    {
        //inventoryManager = FindObjectOfType<InventoryManager>();
        //player = FindObjectOfType<Player>();
    }

    void Update()
    {
        // Close crafting menu if player walks away from bench
        /*if (craftingMenu != null && craftingMenu.activeSelf && player != null)
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
            */
    }

        // ESC key - Close all menus
        /*if (Input.GetKeyDown(KeyCode.Escape))
        {
            //CloseAllMenus();
        }

        // Keyboard shortcuts
        if (Input.GetKeyDown(KeyCode.C));
            //ToggleCharacterMenu();
        
        if (Input.GetKeyDown(KeyCode.K));
            //ToggleCraftingMenu();
    }

    /*public void ToggleCharacterMenu()
    {
        if (characterMenu != null)
        {
            if (characterMenu.activeSelf)
            {
                characterMenu.SetActive(false);
            }
            else
            {
                // Close inventory and crafting when opening character menu
                if (inventoryMenu != null && inventoryMenu.activeSelf && inventoryManager != null)
                {
                    inventoryManager.SendMessage("ToggleInventory");
                }
                if (craftingMenu != null)
                {
                    craftingMenu.SetActive(false);
                    // Re-enable description components
                    if (itemDescriptionImage != null)
                        itemDescriptionImage.enabled = true;
                    if (itemDescriptionBackground != null)
                        itemDescriptionBackground.enabled = true;
                    if (itemDescriptionNameText != null)
                        itemDescriptionNameText.enabled = true;
                    if (itemDescriptionText != null)
                        itemDescriptionText.enabled = true;
                }
                characterMenu.SetActive(true);
            }
        }
    }

    public void OpenCharacterMenu()
    {
        if (characterMenu != null)
        {
            // If already open, close it (toggle behavior)
            if (characterMenu.activeSelf)
            {
                characterMenu.SetActive(false);
            }
            else
            {
                CloseAllMenus();
                characterMenu.SetActive(true);
            }
        }
    }

    public void OpenInventoryMenu()
    {
        if (inventoryManager != null && inventoryMenu != null)
        {
            // If crafting is open, close it (since inventory tab means inventory-only)
            if (craftingMenu != null && craftingMenu.activeSelf)
            {
                craftingMenu.SetActive(false);
                // Re-enable description components
                if (itemDescriptionImage != null)
                    itemDescriptionImage.enabled = true;
                if (itemDescriptionBackground != null)
                    itemDescriptionBackground.enabled = true;
                if (itemDescriptionNameText != null)
                    itemDescriptionNameText.enabled = true;
                if (itemDescriptionText != null)
                    itemDescriptionText.enabled = true;
                
                // Make sure inventory stays open
                if (!inventoryMenu.activeSelf)
                {
                    inventoryManager.SendMessage("OpenInventory");
                }
                return;
            }
            
            // If inventory is already open (and crafting is not open), close inventory (toggle behavior)
            if (inventoryMenu.activeSelf)
            {
                inventoryManager.SendMessage("ToggleInventory");
            }
            else
            {
                // Close character menu
                if (characterMenu != null)
                    characterMenu.SetActive(false);
                
                // Open inventory
                inventoryManager.SendMessage("OpenInventory");
            }
        }
    }

    public void ToggleCraftingMenu()
    {
        // Only allow opening crafting menu if near bench
        if (craftingMenu != null && player != null && player.canOpenCraftingMenu)
        {
            if (craftingMenu.activeSelf)
            {
                craftingMenu.SetActive(false);
                // Re-enable description components when closing
                if (itemDescriptionImage != null)
                    itemDescriptionImage.enabled = true;
                if (itemDescriptionBackground != null)
                    itemDescriptionBackground.enabled = true;
                if (itemDescriptionNameText != null)
                    itemDescriptionNameText.enabled = true;
                if (itemDescriptionText != null)
                    itemDescriptionText.enabled = true;
                
                // Close inventory when closing crafting menu
                if (inventoryMenu != null && inventoryMenu.activeSelf && inventoryManager != null)
                {
                    inventoryManager.SendMessage("ToggleInventory");
                }
            }
            else
            {
                // Close character menu when opening crafting
                if (characterMenu != null)
                    characterMenu.SetActive(false);
                    
                OpenCraftingMenu();
            }
        }
    }

    public void OpenCraftingMenu()
    {
        // Only allow opening crafting menu if near bench
        if (craftingMenu != null && player != null && player.canOpenCraftingMenu)
        {
            // If crafting menu is already open, close it (toggle behavior)
            if (craftingMenu.activeSelf)
            {
                craftingMenu.SetActive(false);
                // Re-enable description components when closing
                if (itemDescriptionImage != null)
                    itemDescriptionImage.enabled = true;
                if (itemDescriptionBackground != null)
                    itemDescriptionBackground.enabled = true;
                if (itemDescriptionNameText != null)
                    itemDescriptionNameText.enabled = true;
                if (itemDescriptionText != null)
                    itemDescriptionText.enabled = true;
                
                // Close inventory when closing crafting menu
                if (inventoryMenu != null && inventoryMenu.activeSelf && inventoryManager != null)
                {
                    inventoryManager.SendMessage("ToggleInventory");
                }
            }
            else
            {
                // Close character menu only
                if (characterMenu != null)
                    characterMenu.SetActive(false);
                
                // Ensure inventory menu is open - use OpenInventory instead of toggle
                if (inventoryMenu != null && !inventoryMenu.activeSelf && inventoryManager != null)
                {
                    inventoryManager.SendMessage("OpenInventory");
                }
                
                // Hide description components
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
    }

    private void CloseAllMenus()
    {
        if (characterMenu != null) characterMenu.SetActive(false);
        if (inventoryMenu != null) inventoryMenu.SetActive(false);
        if (craftingMenu != null) craftingMenu.SetActive(false);
    }

    public void SwitchToTab(int TabID)
    {
        // Call the appropriate toggle method based on TabID
        switch (TabID)
        {
            case 0: // Character Menu
                if (characterMenu != null)
                {
                    if (!characterMenu.activeSelf)
                    {
                        ToggleCharacterMenu();
                    }
                }
                break;
            case 1: // Inventory Menu
                if (inventoryMenu != null && inventoryManager != null)
                {
                    // Close character menu first
                    if (characterMenu != null && characterMenu.activeSelf)
                    {
                        characterMenu.SetActive(false);
                    }
                    
                    // Open inventory if it's not already open
                    if (!inventoryMenu.activeSelf)
                    {
                        inventoryManager.SendMessage("OpenInventory");
                    }
                }
                break;
            case 2: // Crafting Menu
                if (craftingMenu != null)
                {
                    if (!craftingMenu.activeSelf)
                    {
                        ToggleCraftingMenu();
                    }
                }
                break;
        }

        // Update visual tab buttons
        foreach (Image im in TabButtons)
        {
            im.sprite = InactiveTabBG;
            im.rectTransform.sizeDelta = InactiveTabButtonSize;
        }
        
        // Only set active button visual if the corresponding menu is open
        bool shouldActivateButton = false;
        if (TabID == 0 && characterMenu != null && characterMenu.activeSelf)
            shouldActivateButton = true;
        else if (TabID == 1 && inventoryMenu != null && inventoryMenu.activeSelf)
            shouldActivateButton = true;
        else if (TabID == 2 && craftingMenu != null && craftingMenu.activeSelf)
            shouldActivateButton = true;
        
        if (shouldActivateButton && TabID < TabButtons.Length)
        {
            TabButtons[TabID].sprite = ActiveTabBG;
            TabButtons[TabID].rectTransform.sizeDelta = ActiveTabButtonSize;
        }
    }
    */

}
