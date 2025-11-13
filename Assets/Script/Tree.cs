using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : Entity
{
    // Drop wood when destroyed
    [SerializeField] GameObject woodPrefab;
    float currentHealth; // Check when tree has taken damage to drop wood

    void Start()
    {
        currentHealth = health;
    }

    // Drops wood at a random position around the tree
    public void DropWood()
    {
        Vector2 dropPosition = (Vector2)transform.position + Random.insideUnitCircle * 0.5f;  // Drop within 0.5 units radius of the tree
        Instantiate(woodPrefab, dropPosition, Quaternion.identity);
    }

    void Update()
    {
        // For testing: Destroy tree when 'K' is pressed
        if (Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(1);
        }
        // if tree taken damage, drop wood
            if (health < currentHealth)
            {
                DropWood();
                currentHealth = health;
            }

            // Check if tree is destroyed, destroy and drop wood
            if (health <= 0)
            {
                Die();
            }
    }
}
