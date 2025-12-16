using System;
using UnityEngine;
using Inventory.Model;

// allows CraftingBench.cs to take recipes and reads from the list of Ingredients.
// the output can be manipulated as well, but the initial values are 1.
[CreateAssetMenu(menuName = "Crafting/Recipe")]
public class CraftingRecipe : ScriptableObject
{
    [Serializable]
    public struct Ingredient
    {
        public ItemSO item;
        public int amount;
    }

    public ItemSO result;
    public int resultQuantity = 1;
    public Ingredient[] ingredients;
}
