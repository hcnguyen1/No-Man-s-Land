using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftingUIController : MonoBehaviour
{
    [Header("References")]
    public CraftingBench bench;
    public InventoryManager inventoryManager;
    public CraftingRecipe[] recipes;
    
    [Header("UI Elements")]
    public GameObject craftingMenu;
    public Transform recipeListContainer;  // Parent object for recipe buttons (Content)
    public GameObject recipeButtonPrefab;  // Prefab for each recipe button
    
    private CraftingRecipe selectedRecipe;

    void Start()
    {
        // Delay refresh to let InventoryManager.Start() run first
        Invoke("RefreshRecipeList", 0.1f);
        SubscribeToItemSlots();
    }
    
    void SubscribeToItemSlots()
    {
        if (inventoryManager == null) return;
        
        foreach (var slot in inventoryManager.itemSlot)
        {
            if (slot != null)
                slot.OnItemClicked += OnInventoryItemClicked;
        }
    }
    
    void OnInventoryItemClicked(InventoryManager manager)
    {
        // When crafting menu is open, clicking inventory items could show info
        // For now, we just let the recipe selection handle the display
    }

    void Update()
    {
        // Removed constant refresh - only refresh when needed
    }

    public void RefreshRecipeList()
    {
        if (recipeListContainer == null || recipes == null) return;
        
        // Clear existing buttons
        foreach (Transform child in recipeListContainer)
        {
            Destroy(child.gameObject);
        }
        
        // Create button for each recipe
        foreach (var recipe in recipes)
        {
            if (recipe == null) continue;
            
            // Check if player has ingredients
            bool canCraft = HasIngredients(recipe);
            
            GameObject buttonObj = Instantiate(recipeButtonPrefab, recipeListContainer);
            Button button = buttonObj.GetComponent<Button>();
            CanvasGroup canvasGroup = buttonObj.GetComponent<CanvasGroup>();
            
            // Add CanvasGroup if it doesn't exist
            if (canvasGroup == null)
            {
                canvasGroup = buttonObj.AddComponent<CanvasGroup>();
            }
            
            // Find child components in the prefab
            Image resultIcon = buttonObj.transform.Find("ResultIcon")?.GetComponent<Image>();
            TextMeshProUGUI recipeName = buttonObj.transform.Find("RecipeNameText")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI ingredientsText = buttonObj.transform.Find("IngredientsText")?.GetComponent<TextMeshProUGUI>();
            
            // Set result icon
            if (resultIcon != null && recipe.result.icon != null)
            {
                resultIcon.sprite = recipe.result.icon;
            }
            
            // Set recipe name
            if (recipeName != null)
            {
                recipeName.text = recipe.result.itemName;
            }
            
            // Build ingredients text
            if (ingredientsText != null)
            {
                string ingredients = "Required: ";
                for (int i = 0; i < recipe.ingredients.Length; i++)
                {
                    if (recipe.ingredients[i].item != null)
                    {
                        ingredients += recipe.ingredients[i].item.itemName + " x" + recipe.ingredients[i].amount;
                        if (i < recipe.ingredients.Length - 1) ingredients += ", ";
                    }
                }
                ingredientsText.text = ingredients;
            }
            
            // Dim button if can't craft
            if (canvasGroup != null)
            {
                canvasGroup.alpha = canCraft ? 1f : 0.5f;
            }
            
            if (button != null)
            {
                button.interactable = canCraft;
                button.onClick.AddListener(() => CraftRecipe(recipe));
            }
        }
    }

    bool HasIngredients(CraftingRecipe recipe)
    {
        if (inventoryManager == null || recipe == null) return false;
        
        foreach (var ingredient in recipe.ingredients)
        {
            if (ingredient.item == null) continue;
            
            int totalAmount = 0;
            foreach (var slot in inventoryManager.itemSlot)
            {
                if (slot.itemName == ingredient.item.itemName)
                {
                    totalAmount += slot.quantity;
                }
            }
            
            if (totalAmount < ingredient.amount)
                return false;
        }
        
        return true;
    }

    void CraftRecipe(CraftingRecipe recipe)
    {
        if (recipe == null || inventoryManager == null) return;
        
        if (!HasIngredients(recipe))
        {
            Debug.Log("Not enough ingredients!");
            return;
        }
        
        // Remove ingredients from inventory
        foreach (var ingredient in recipe.ingredients)
        {
            if (ingredient.item == null) continue;
            
            int amountToRemove = ingredient.amount;
            foreach (var slot in inventoryManager.itemSlot)
            {
                if (slot.itemName == ingredient.item.itemName && amountToRemove > 0)
                {
                    int removeFromSlot = Mathf.Min(slot.quantity, amountToRemove);
                    slot.AddQuantity(-removeFromSlot);
                    amountToRemove -= removeFromSlot;
                }
            }
        }
        
        // Add result to inventory
        inventoryManager.AddItem(
            recipe.result.itemName,
            recipe.resultQuantity,
            recipe.result.icon,
            recipe.result.itemDescription
        );
        
        Debug.Log($"Crafted {recipe.result.itemName} x{recipe.resultQuantity}");
        
        // Refresh UI
        RefreshRecipeList();
    }

    public void ToggleMenu()
    {
        if (craftingMenu != null)
        {
            craftingMenu.SetActive(!craftingMenu.activeSelf);
            if (craftingMenu.activeSelf)
                RefreshRecipeList();
        }
    }
}
