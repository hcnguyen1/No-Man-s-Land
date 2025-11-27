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
    private Transform recipeContent; // The Content of the ScrollView (RecipeScrollView > Viewport > Content)

    [SerializeField]
    private GameObject recipeButtonPrefab; // Prefab for each recipe button/panel

    [SerializeField]
    private TextMeshProUGUI recipeNameText; // Shows recipe name

    [SerializeField]
    private TextMeshProUGUI ingredientsText; // Shows ingredients

    [SerializeField]
    private Image resultIcon; // Shows the result item image

    private CraftingBench currentBench;
    private CraftingRecipe selectedRecipe;
    private List<Button> recipeButtons = new List<Button>();

    void Start()
    {
        // No separate craft button - crafting happens when clicking recipe buttons
    }

    public void SetCraftingBench(CraftingBench bench)
    {
        currentBench = bench;
        RefreshRecipeList();
    }

    public void RefreshRecipeList()
    {
        if (currentBench == null)
            return;

        // Clear existing recipe buttons
        foreach (Transform child in recipeContent)
        {
            Destroy(child.gameObject);
        }
        recipeButtons.Clear();

        // Create buttons for each recipe
        List<CraftingRecipe> recipes = currentBench.GetAvailableRecipes();
        foreach (CraftingRecipe recipe in recipes)
        {
            GameObject buttonObj = Instantiate(recipeButtonPrefab, recipeContent);
            Button button = buttonObj.GetComponent<Button>();
            TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();

            if (buttonText != null)
                buttonText.text = recipe.result.Name;

            // Check if recipe can be crafted and set button color
            bool canCraft = currentBench.CanCraft(recipe);
            Image buttonImage = buttonObj.GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImage.color = canCraft ? Color.white : new Color(0.5f, 0.5f, 0.5f); // Gray if can't craft
            }
            
            button.interactable = canCraft;
            
            // Clicking button selects recipe and if craftable, craft it
            button.onClick.AddListener(() => 
            {
                SelectRecipe(recipe);
                if (currentBench.CanCraft(recipe))
                    AttemptCraft();
            });
            recipeButtons.Add(button);
        }

        // Select first recipe by default
        if (recipes.Count > 0)
            SelectRecipe(recipes[0]);
    }

    public void SelectRecipe(CraftingRecipe recipe)
    {
        selectedRecipe = recipe;

        if (recipe == null)
        {
            recipeNameText.text = "";
            ingredientsText.text = "";
            if (resultIcon != null)
                resultIcon.sprite = null;
            return;
        }

        // Display recipe name
        recipeNameText.text = recipe.result.Name;

        // Display result icon
        if (resultIcon != null)
            resultIcon.sprite = recipe.result.ItemImage;

        // Display ingredients
        string details = $"Result: x{recipe.resultQuantity}\n\n";
        details += "Ingredients:\n";
        
        foreach (var ingredient in recipe.ingredients)
        {
            details += $"  • {ingredient.item.Name} x{ingredient.amount}\n";
        }

        // Check if can craft and show missing ingredients
        bool canCraft = currentBench.CanCraft(recipe);

        if (!canCraft)
        {
            details += "\n<color=red>Missing:\n";
            List<string> missing = currentBench.GetMissingIngredients(recipe);
            foreach (var item in missing)
            {
                details += $"  • {item}\n";
            }
            details += "</color>";
        }

        ingredientsText.text = details;
    }

    public void AttemptCraft()
    {
        if (selectedRecipe == null || currentBench == null)
            return;

        bool success = currentBench.TryCraft(selectedRecipe);
        
        if (success)
        {
            RefreshRecipeList();
            Debug.Log($"Successfully crafted {selectedRecipe.result.Name}!");
        }
        else
        {
            Debug.LogWarning("Failed to craft!");
        }
    }
}

