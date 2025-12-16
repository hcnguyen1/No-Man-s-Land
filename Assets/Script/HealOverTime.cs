using System.Collections;
using UnityEngine;

// generally not used in this project but the idea is there for when we want to heal over time rather than all at once.
// The feature can be used if we want to create a potion that needs this specifically. 
public class HealOverTime : MonoBehaviour
{
    private Coroutine healingCoroutine;

    public void StartHealing(float healPerTick, float tickInterval, int totalTicks, Player player)
    {
        // Stop any existing healing
        if (healingCoroutine != null)
        {
            StopCoroutine(healingCoroutine);
        }

        // Start new healing coroutine
        healingCoroutine = StartCoroutine(HealCoroutine(healPerTick, tickInterval, totalTicks, player));
    }

    private IEnumerator HealCoroutine(float healPerTick, float tickInterval, int totalTicks, Player player)
    {
        int ticksRemaining = totalTicks;

        while (ticksRemaining > 0 && player != null)
        {
            // Wait for the tick interval
            yield return new WaitForSeconds(tickInterval);

            // Heal the player
            player.health += healPerTick;
            player.health = Mathf.Clamp(player.health, 0, player.maxHealth);

            ticksRemaining--;

            // Stop if player is at max health
            if (player.health >= player.maxHealth)
            {
                break;
            }
        }

        // Healing complete
        healingCoroutine = null;
    }

    // Optional: Stop healing if the player dies or component is disabled
    private void OnDisable()
    {
        if (healingCoroutine != null)
        {
            StopCoroutine(healingCoroutine);
            healingCoroutine = null;
        }
    }
}
