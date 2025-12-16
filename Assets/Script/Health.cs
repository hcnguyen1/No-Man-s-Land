using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Health.cs has its own value to manipulate the players health and whether they take damage,
// but it can also be placed in player.cs for simplicity. 
// we can get the players current health, max health, or call Die() if player receives fatal damage. 
public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void AddHealth(int amount)
    {
        // Don't heal if already at max health
        if (currentHealth >= maxHealth)
        {
            return;
        }
        
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
    }

    public void TakeDamage(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth - amount, 0, maxHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    private void Die()
    {
        Die();
    }
}
