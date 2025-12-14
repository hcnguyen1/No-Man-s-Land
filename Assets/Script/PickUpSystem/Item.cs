using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inventory.Model;
using System;

public class Item : MonoBehaviour
{
    [field: SerializeField]
    public ItemSO InventoryEntry {get; set; } // Changed to set; so it can be assigned at runtime

    [field: SerializeField]
    public int Quantity {get; set; } = 1;

    [SerializeField]
    private AudioSource audioSource;
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
    }

    public void DestroyItem()
    {
        GetComponent<Collider2D>().enabled = false;
        StartCoroutine(AnimateItemPickup());
    }

    private IEnumerator AnimateItemPickup() // once you pick up the item, the size will change and the item on the ground will be destroyed.
    {
        if (audioSource != null)
        {
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
