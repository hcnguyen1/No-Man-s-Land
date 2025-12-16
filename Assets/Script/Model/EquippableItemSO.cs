using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// This class is the equippable item SO where it takes the ItemSOs classes and also enforces whether the item can be equipped upon right click.

namespace Inventory.Model
{
    [CreateAssetMenu]
    public class EquippableItemSO : ItemSO, IDestroyableItem, IItemAction // Idestroyable allows the item to be destroyed as well as allow Item to perform inventory actions. 
    {
        [SerializeField]
        private List<ModifierData> modifiersData = new List<ModifierData>();

        public string ActionName => "Equip";

        [field: SerializeField]
        public AudioClip actionSFX { get; private set; }

        public List<ModifierData> ModifiersData => modifiersData;

        
        public bool PerformAction(GameObject character, List<ItemParameter> itemState = null) // will check if the item is indeed equippable. 
        {
            AgentWeapon weaponSystem = character.GetComponent<AgentWeapon>();
            if (weaponSystem != null)
            {
                weaponSystem.SetWeapon(this, itemState == null ? DefaultParametersList : itemState);
                return true;
            }
            return false;
        }
    }
}

