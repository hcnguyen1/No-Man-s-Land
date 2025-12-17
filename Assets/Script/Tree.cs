using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inventory.Model;

public class Tree : Entity
{
    // Item to drop when destroyed
    [SerializeField] ItemSO itemToDrop;
    private float currentHealth; // Check when tree has taken damage to drop item

    // Sound effects
    [SerializeField] private AudioClip choppingSFX;
    [SerializeField] private AudioClip treeFallSFX;

    void Start()
    {
        currentHealth = health;
    }

    protected override void OnZeroHealth()
    {
        Debug.Log("[Tree] OnZeroHealth called - Tree is dying");
        // Drop items before dying
        DropItem();
        
        // Play tree fall sound at position (independent of GameObject)
        if (treeFallSFX != null)
        {
            AudioSource.PlayClipAtPoint(treeFallSFX, transform.position);
        }
        
        // Now call Die to destroy the tree
        Die();
    }

    // Drops item at a random position around the tree
    public void DropItem()
    {
        if (itemToDrop == null)
        {
            Debug.LogWarning("[Tree] DropItem called but itemToDrop is NULL! Check Inspector.");
            return;
        }

        Debug.Log($"[Tree] Dropping item: {itemToDrop.Name} at position {transform.position}");

        // Create item GameObject manually like Cow does
        GameObject droppedItem = new GameObject("DroppedItem");
        droppedItem.transform.position = (Vector2)transform.position + Random.insideUnitCircle * 0.5f;
        droppedItem.transform.localScale = Vector3.one * 0.5f; 
        
        // Add sprite renderer
        SpriteRenderer sr = droppedItem.AddComponent<SpriteRenderer>();
        sr.sprite = itemToDrop.ItemImage;
        sr.sortingLayerName = "Foreground"; // Ensure item appears above ground
        
        // Add collider
        CircleCollider2D col = droppedItem.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = 0.5f;
        
        // Add AudioSource for pickup sound
        AudioSource itemAudioSource = droppedItem.AddComponent<AudioSource>();
        itemAudioSource.playOnAwake = false;
        
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
        // Play chopping sound when tree takes damage (but don't drop item yet)
        if (health < currentHealth)
        {
            currentHealth = health;

            // Play chopping sound when tree takes damage
            if (audioSource != null && choppingSFX != null)
            {
                audioSource.PlayOneShot(choppingSFX);
            }
        }
    }
}
