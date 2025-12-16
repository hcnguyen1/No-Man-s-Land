using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


// This class is the consumable item SO where it takes the ItemSOs classes and also enforces whether the item can be consumed upon right click.
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



        // performs the action of cycling through the stats that can affect the character. 
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
                // Check if this is a hunger modifier
                else if (data.statModifier is CharacterStatHungerModifierSO)
                {
                    Player player = character.GetComponent<Player>();
                    if (player != null && player.hunger < player.maxHunger)
                    {
                        canPerformAction = true;
                        break;
                    }
                }
                // Check if this is a thirst modifier
                else if (data.statModifier is CharacterStatThirstModifierSO)
                {
                    Player player = character.GetComponent<Player>();
                    if (player != null && player.thirst < player.maxThirst)
                    {
                        canPerformAction = true;
                        break;
                    }
                }
                else
                {
                    // For other modifiers, always allow
                    canPerformAction = true;
                    break;
                }
            }
            
            // Only consume if the action can be performed
            if (!canPerformAction)
            {
                return false;
            }
            // for EVERY stat that can affect the player, we want to cycle and make sure it all applies. 
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
    public class ModifierData // this is the data that can be modified by the class by calling the statmodifierSO.
    {
        public CharacterStatModifierSO statModifier;
        public float value;
    }
}

