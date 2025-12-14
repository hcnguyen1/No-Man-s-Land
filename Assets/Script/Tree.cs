using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inventory.Model;

public class Tree : Entity
{
    // Drop wood when destroyed
    [SerializeField] ItemSO woodItem;
    float currentHealth; // Check when tree has taken damage to drop wood

    // Sound effects
    [SerializeField] private AudioClip choppingSFX;
    [SerializeField] private AudioClip treeFallSFX;

    void Start()
    {
        currentHealth = health;
    }

    // Drops wood at a random position around the tree
    public void DropWood()
    {
        if (woodItem == null)
            return;

        // Create item GameObject manually like Cow does
        GameObject droppedWood = new GameObject("DroppedWood");
        droppedWood.transform.position = (Vector2)transform.position + Random.insideUnitCircle * 0.5f;
        
        // Add sprite renderer
        SpriteRenderer sr = droppedWood.AddComponent<SpriteRenderer>();
        sr.sprite = woodItem.ItemImage;
        
        // Add collider
        CircleCollider2D col = droppedWood.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = 0.5f;
        
        // Add Item script and initialize it
        Item itemScript = droppedWood.AddComponent<Item>();
        itemScript.Initialize(woodItem, 1);
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

                // Play chopping sound when tree takes damage
                if (audioSource != null && choppingSFX != null)
                {
                    audioSource.PlayOneShot(choppingSFX);
                }
            }

            // Check if tree is destroyed, destroy and drop wood
            if (health <= 0)
            {
                // Play tree fall sound when tree dies
                if (audioSource != null && treeFallSFX != null)
                {
                    audioSource.PlayOneShot(treeFallSFX);
                }
                Die();
            }
    }
}
