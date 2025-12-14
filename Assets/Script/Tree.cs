using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inventory.Model;

public class Tree : Entity
{
    // Item to drop when destroyed
    [SerializeField] ItemSO itemToDrop;
    float currentHealth; // Check when tree has taken damage to drop item

    // Sound effects
    [SerializeField] private AudioClip choppingSFX;
    [SerializeField] private AudioClip treeFallSFX;

    void Start()
    {
        currentHealth = health;
    }

    // Drops item at a random position around the tree
    public void DropItem()
    {
        if (itemToDrop == null)
            return;

        // Create item GameObject manually like Cow does
        GameObject droppedItem = new GameObject("DroppedItem");
        droppedItem.transform.position = (Vector2)transform.position + Random.insideUnitCircle * 0.5f;
        
        // Add sprite renderer
        SpriteRenderer sr = droppedItem.AddComponent<SpriteRenderer>();
        sr.sprite = itemToDrop.ItemImage;
        
        // Add collider
        CircleCollider2D col = droppedItem.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = 0.5f;
        
        // Add Item script and initialize it
        Item itemScript = droppedItem.AddComponent<Item>();
        itemScript.Initialize(itemToDrop, 1);
    }

    void Update()
    {
        // For testing: Destroy tree when 'K' is pressed
        if (Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(1);
        }
        // if tree taken damage, drop item
            if (health < currentHealth)
            {
                DropItem();
                currentHealth = health;

                // Play chopping sound when tree takes damage
                if (audioSource != null && choppingSFX != null)
                {
                    audioSource.PlayOneShot(choppingSFX);
                }
            }

            // Check if tree is destroyed, destroy and drop item
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
