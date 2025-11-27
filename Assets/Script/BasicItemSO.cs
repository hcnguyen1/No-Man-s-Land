using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inventory.Model;

namespace Inventory.Model
{
    [CreateAssetMenu(menuName = "Inventory/Basic Item")]
    public class BasicItemSO : ItemSO
    {
        // Basic items like herbs, rocks, etc.
        // No durability, no special actions
        // These are typically crafting ingredients
    }
}


