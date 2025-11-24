using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Inventory.Model
{
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
        public List<ItemParameter> DefaultParametersList { get; private set; }
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

