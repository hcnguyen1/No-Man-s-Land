using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inventory.Model;

namespace Inventory.Model
{
    // The class doesn't really have any functions aside from being called a BasicItem because we want to specify what 
    // each item is, this would be to specify a normal item that cannot be consumed or equipped to. 

    [CreateAssetMenu(menuName = "Inventory/Basic Item")]
    public class BasicItemSO : ItemSO
    {
        // Basic items like herbs, rocks, etc.
        // No durability, no special actions
        // These are typically crafting ingredients
    }
}


