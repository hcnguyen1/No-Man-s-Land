using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory.Model

// this class is the ItemParameterSO where it initializes the parameter for the chosen item as well as their name for serializefield
// the functions then are part of scriptable object and will be pulled from statsmodifiers or durability depending on which 
// type of ItemSO they are. e.g., ConsumableItemSO or EquippableItemSO.
{
    [CreateAssetMenu]
    public class ItemParameterSO : ScriptableObject
    {
        [field: SerializeField]
        public string ParameterName {get; private set; }
        
    }
}

