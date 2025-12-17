using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inventory.Model;

public class Stone : Entity
{
    [SerializeField] ItemSO item;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip miningSFX;
    [SerializeField] private AudioClip stoneFallSFX;

    private float currentHealth; // To track health for StoneHit sound
    private void Update()
    {
        TestingDamage();
        PlayMiningSFX();
    }

    private void TestingDamage()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(1);
        }
    }

    protected override void OnZeroHealth()
    {
        DropItem();
        PlayStoneFallSFX();
        Die();
    }

    public void DropItem()
    {
        if (item == null)
        {
            Debug.LogWarning("[Stone] DropItem called but item is NULL! Check Inspector.");
            return;
        }

        Debug.Log($"[Stone] Dropping item: {item.Name} at position {transform.position}");
        // Create item GameObject manually like Cow does
        GameObject droppedItem = new GameObject("DroppedItem");
        droppedItem.transform.position = (Vector2)transform.position + Random.insideUnitCircle * 0.5f;
        droppedItem.transform.localScale = Vector3.one * 0.5f; 
        
        // Add sprite renderer
        SpriteRenderer sr = droppedItem.AddComponent<SpriteRenderer>();
        sr.sprite = item.ItemImage;
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
        itemScript.Initialize(item, 1);
    }

    private void PlayStoneFallSFX()
    {
        if (miningSFX != null)
        {
            AudioSource.PlayClipAtPoint(miningSFX, transform.position);
        }
    }

    private void PlayMiningSFX()
    {
        // Play mining sound only if health has decreased
        if(health < currentHealth)
        {
            if (miningSFX != null && audioSource != null)
            {
                AudioSource.PlayClipAtPoint(miningSFX, transform.position);
            }
            currentHealth = health;
        }
    }

}
