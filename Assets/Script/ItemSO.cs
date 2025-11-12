using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu] // can create instances of this SO 
public class ItemSO : ScriptableObject  // this is not monobehavior but can persist through playthroughs. 
{
    // cannot use start and update and will have to use other scripts 

    public string itemName;
    public StatToChange statToChange = new StatToChange();
    public int amountToChangeStat;

    public AttributeToChange attributeToChange = new AttributeToChange();
    public int amountToChangeAttribute;


    public bool UseItem()
    {
        if (statToChange == StatToChange.health)
        {
        }

        if (statToChange == StatToChange.stamina)
        { 
        }

        if (statToChange == StatToChange.none)
        {
        }

        return false; // just filler to make function work properly as a boolean. 
    }

    public enum StatToChange
    {
        none,
        health,
        stamina
    };

    public enum AttributeToChange
    {
        none,
        speed,
        defense,
        attack,
        stamina
    };
}
