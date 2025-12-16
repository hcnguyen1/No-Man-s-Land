using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Inventory.Model
{
    // while inventorySO utilizes and manipulates items within its inventory, the individual itemSO allows us to dive into the details and change 
    // the name, description, how many stacks the item can have, what kind of sounds will be played, and other fields like price.
    // most importantly, its the base ItemSO so we have derrived 2 types of ItemSO's from this class, namely ConsumableItemS0 and EquippableItemSO. 
    public abstract class ItemSO : ScriptableObject
    {
        [field: SerializeField] // field: serializes field of a property
        public bool IsStackable { get; set; } //allows us to see if item is stackable 

        public int ID => GetInstanceID();

        [field: SerializeField]
        public int MaxStackSize { get; set; } = 1;
        [field: SerializeField]
        public string Name { get; set; }
        [field: SerializeField]
        [field: TextArea]
        public string Description { get; set; }

        [field: SerializeField]
        public Sprite ItemImage { get; set; }

        [field: SerializeField]
        public AudioClip PickupSound { get; set; } // Sound to play when item is picked up

        [field: SerializeField]
        public List<ItemParameter> DefaultParametersList { get; private set; }

        [field: SerializeField]
        public int Price { get; set; } = 0; // Price for shop system
    }

    [Serializable]
    public struct ItemParameter : IEquatable<ItemParameter>
    {
        public ItemParameterSO itemParameter;
        public float value;

        public bool Equals(ItemParameter other)
        {
            return other.itemParameter == itemParameter;
        }
    }
}

