using System;
using UnityEngine;

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
