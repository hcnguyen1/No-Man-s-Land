using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Inventory.Model
{
    [CreateAssetMenu]
    public class ConsumableItemSO : ItemSO, IDestroyableItem, IItemAction
    {
        [SerializeField]
        private List<ModifierData> modifiersData = new List<ModifierData>();

        public string ActionName => "Consume";

        [field: SerializeField]
        public AudioClip actionSFX { get; private set; }



        
        public bool PerformAction(GameObject character, List<ItemParameter> itemState = null)
        {
            // Check if the item would have any effect before consuming it
            bool canPerformAction = false;
            
            foreach (ModifierData data in modifiersData)
            {
                // Check if this is a health modifier
                if (data.statModifier is CharacterStatHealthModifierSO)
                {
                    Health health = character.GetComponent<Health>();
                    if (health != null && health.GetCurrentHealth() < health.GetMaxHealth())
                    {
                        canPerformAction = true;
                        break;
                    }
                }
                else
                {
                    // For non-health modifiers, always allow
                    canPerformAction = true;
                    break;
                }
            }
            
            // Only consume if the action can be performed
            if (!canPerformAction)
            {
                Debug.Log("Cannot consume item - already at full health!");
                return false;
            }
            
            foreach (ModifierData data in modifiersData)
            {
                data.statModifier.AffectCharacter(character, data.value);
            }
            return true;
        }
    }

    public interface IDestroyableItem {} // this will be an interface where we can destroy the item


    public interface IItemAction // gives us options when encountering specific equippable items. 
    {
        public string ActionName { get; } // something like restore health or it gives a bonus
        public AudioClip actionSFX { get; }
        bool PerformAction(GameObject character, List<ItemParameter> itemState);
    }

    [Serializable]
    public class ModifierData
    {
        public CharacterStatModifierSO statModifier;
        public float value;
    }
}

