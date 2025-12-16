using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inventory.Model;
using System;

// This class is the Item class which is very important to the structure of the inventory system. 
// the items sprite and other details need to be updated through this class in order to reach 
// the UI. It is also in charge of manipulating the item's size via local transform as well as the sound queue.
public class Item : MonoBehaviour
{
    [field: SerializeField]
    public ItemSO InventoryEntry {get; set; } // Changed to set; so it can be assigned at runtime

    [field: SerializeField]
    public int Quantity {get; set; } = 1;

    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip pickupSound; // Audio clip for pickup sound
    [SerializeField]
    private float duration = 0.3f;

    private void Start()
    {
        UpdateSprite();
    }

    private void UpdateSprite()
    {
        if (InventoryEntry != null)
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = InventoryEntry.ItemImage;
            }
        }
    }

    // Call this after setting InventoryEntry at runtime
    public void Initialize(ItemSO itemSO, int quantity)
    {
        InventoryEntry = itemSO;
        Quantity = quantity;
        UpdateSprite();
        
        // Set pickup sound from ItemSO if not already set
        if (pickupSound == null && itemSO != null && itemSO.PickupSound != null)
        {
            pickupSound = itemSO.PickupSound;
        }
    }

    public void DestroyItem()
    {
        GetComponent<Collider2D>().enabled = false;
        StartCoroutine(AnimateItemPickup());
    }

    private IEnumerator AnimateItemPickup() // once you pick up the item, the size will change and the item on the ground will be destroyed.
    {
        // Get AudioSource if not assigned
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
        
        // Play pickup sound (works even without AudioSource on this object)
        if (pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
        }
        else if (audioSource != null && audioSource.clip != null)
        {
            // Fallback to AudioSource if it has a clip assigned
            audioSource.Play();
        }
        
        Vector3 startScale = transform.localScale;
        Vector3 endScale = Vector3.zero;
        float currentTime = 0;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScale, endScale, currentTime / duration);
            yield return null;
        }
        Destroy(gameObject);
    }
}
