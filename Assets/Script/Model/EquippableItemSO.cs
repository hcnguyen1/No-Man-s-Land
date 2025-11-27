using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// main .cs script file that lets us equip items, but they all will implement other .cs files related to the weapon/equipment being equipped. 

namespace Inventory.Model
{
    [CreateAssetMenu]
    public class EquippableItemSO : ItemSO, IDestroyableItem, IItemAction
    {
        public string ActionName => "Equip";

        [field: SerializeField]
        public AudioClip actionSFX { get; private set; }

        
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

