using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
