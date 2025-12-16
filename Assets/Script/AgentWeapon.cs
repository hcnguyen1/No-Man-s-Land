using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inventory.Model;

// Referenced from a youtube video, this class basically allows us to equip, set, and unequip items as well as apply those bonuses
// from the EquippableItemSO. The classes functions are pretty simple, either to apply or remove the stats.
public class AgentWeapon : MonoBehaviour
{
    [SerializeField]
    private EquippableItemSO weapon;

    [SerializeField]
    private InventorySO inventoryData;

    [SerializeField]
    private List<ItemParameter> parametersToModify;

    [SerializeField]
    private List<ItemParameter> itemCurrentState;

    // this is the set method where it takes the state of the weapon and if you remove it, will be back in inventory.
    public void SetWeapon(EquippableItemSO weaponItemSO, List<ItemParameter> itemState) 
    {
        // Remove stat bonuses from old weapon before unequipping
        if (weapon != null)
        {
            RemoveWeaponStatBonuses(weapon);
            inventoryData.AddItem(weapon, 1, itemCurrentState);
        }

        this.weapon = weaponItemSO;
        this.itemCurrentState = new List<ItemParameter>(itemState);
        ModifyParameters();
        
        // Apply stat bonuses from new weapon
        if (weapon != null)
        {
            ApplyWeaponStatBonuses(weapon);
        }
    }

    private void ApplyWeaponStatBonuses(EquippableItemSO weaponItem)
    {
        if (weaponItem.ModifiersData == null) return;
        
        foreach (ModifierData data in weaponItem.ModifiersData)
        {
            data.statModifier.AffectCharacter(gameObject, data.value);
        }
    }

    private void RemoveWeaponStatBonuses(EquippableItemSO weaponItem)
    {
        if (weaponItem.ModifiersData == null) return;
        
        foreach (ModifierData data in weaponItem.ModifiersData)
        {
            // Apply negative value to remove the bonus
            data.statModifier.AffectCharacter(gameObject, -data.value);
        }
    }

    private void ModifyParameters()
    {
        foreach (var parameter in parametersToModify)
        {
            if (itemCurrentState.Contains(parameter)) // if the list contains the parameter, it will check the current state of the parameter and change it, either by adding or substracting.
            {
                int index = itemCurrentState.IndexOf(parameter);
                float newValue = itemCurrentState[index].value + parameter.value;
                itemCurrentState[index] = new ItemParameter
                {
                    itemParameter = parameter.itemParameter,
                    value = newValue
                };
            }
        }
    }
}
