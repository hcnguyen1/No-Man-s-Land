using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Inventory.Model;
using Inventory;

public class CraftingUI : MonoBehaviour
{
    [SerializeField]
    private GameObject recipeButton; // The RecipeButton GameObject in the scene

    private CraftingBench currentBench;
    private CraftingRecipe selectedRecipe;

    void Start()
    {
        // Find the text components in RecipeButton
        if (recipeButton != null)
        {
            recipeButton.SetActive(false); // Hide initially
        }
    }

    public void SetCraftingBench(CraftingBench bench)
    {
        currentBench = bench;
        DisplayRecipes();
    }

    public void DisplayRecipes()
    {
        if (currentBench == null || recipeButton == null)
            return;

        List<CraftingRecipe> recipes = currentBench.GetAvailableRecipes();
        
        if (recipes.Count > 0)
        {
            // Show the button and display first recipe
            recipeButton.SetActive(true);
            DisplayRecipe(recipes[0]);
        }
        else
        {
            recipeButton.SetActive(false);
        }
    }

    private void DisplayRecipe(CraftingRecipe recipe)
    {
        if (recipe == null || recipeButton == null)
            return;

        selectedRecipe = recipe;

        // Find text components in RecipeButton
        TextMeshProUGUI nameText = recipeButton.transform.Find("RecipeNameText")?.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI ingredientsText = recipeButton.transform.Find("IngredientsText")?.GetComponent<TextMeshProUGUI>();
        Image resultIcon = recipeButton.transform.Find("ResultIcon")?.GetComponent<Image>();

        if (nameText != null)
            nameText.text = recipe.result.Name;

        if (resultIcon != null)
            resultIcon.sprite = recipe.result.ItemImage;

        if (ingredientsText != null)
        {
            bool canCraft = currentBench.CanCraft(recipe);
            
            if (canCraft)
            {
                string details = $"Result: x{recipe.resultQuantity}\n\nIngredients:\n";
                foreach (var ingredient in recipe.ingredients)
                {
                    details += $"• {ingredient.item.Name} x{ingredient.amount}\n";
                }
                ingredientsText.text = details;
            }
            else
            {
                string details = "<color=red>Missing:\n";
                List<string> missing = currentBench.GetMissingIngredients(recipe);
                foreach (var item in missing)
                {
                    details += $"• {item}\n";
                }
                details += "</color>";
                ingredientsText.text = details;
            }
        }

        // Setup craft button
        Button craftBtn = recipeButton.GetComponent<Button>();
        if (craftBtn != null)
        {
            craftBtn.onClick.RemoveAllListeners();
            craftBtn.onClick.AddListener(() => AttemptCraft());
            craftBtn.interactable = currentBench.CanCraft(recipe);
        }
    }

    public void AttemptCraft()
    {
        if (selectedRecipe == null || currentBench == null)
            return;

        bool success = currentBench.TryCraft(selectedRecipe);
        
        if (success)
        {
            DisplayRecipes(); // Refresh to update craftability
            Debug.Log($"Successfully crafted {selectedRecipe.result.Name}!");
        }
        else
        {
            Debug.LogWarning("Failed to craft!");
        }
    }
}

