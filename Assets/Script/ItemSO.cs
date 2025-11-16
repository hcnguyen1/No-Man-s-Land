using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu] // can create instances of this SO 
public class ItemSO : ScriptableObject  // this is not monobehavior but can persist through playthroughs. 
{
    // cannot use start and update and will have to use other scripts 

    public string itemName;
    public Sprite icon;
    [TextArea] public string itemDescription;
    public StatToChange statToChange = new StatToChange();
    public int amountToChangeStat;

    [Header("Heal Over Time Settings")]
    public bool isHealOverTime = false;
    public float healPerTick = 5f;  // HP healed per tick
    public float tickInterval = 1f;  // Time between ticks (in seconds)
    public int totalTicks = 10;  // Total number of healing ticks

    public AttributeToChange attributeToChange = new AttributeToChange();
    public int amountToChangeAttribute;

    public bool UseItem()
    {
        if (statToChange == StatToChange.health)
        {
            Player player = GameObject.FindWithTag("Player")?.GetComponent<Player>();
            if (player != null)
            {
                if (isHealOverTime)
                {
                    // Start heal over time
                    HealOverTime healComponent = player.GetComponent<HealOverTime>();
                    if (healComponent == null)
                    {
                        healComponent = player.gameObject.AddComponent<HealOverTime>();
                    }
                    healComponent.StartHealing(healPerTick, tickInterval, totalTicks, player);
                    Debug.Log($"Started healing over time: {healPerTick} HP every {tickInterval}s for {totalTicks} ticks");
                }
                else
                {
                    // Instant heal
                    player.health += amountToChangeStat;
                    player.health = Mathf.Clamp(player.health, 0, player.maxHealth);
                    Debug.Log($"Restored {amountToChangeStat} health. Current health: {player.health}");
                }
                return true;
            }
        }

        if (statToChange == StatToChange.stamina)
        { 
            // Stamina logic can be added here later
        }

        if (statToChange == StatToChange.none)
        {
            return false;
        }

        return false;
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
