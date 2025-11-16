using System.Collections;
using UnityEngine;

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

            Debug.Log($"Healing tick: +{healPerTick} HP. Current health: {player.health}. Ticks remaining: {ticksRemaining}");

            // Stop if player is at max health
            if (player.health >= player.maxHealth)
            {
                Debug.Log("Player at max health, stopping healing.");
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
