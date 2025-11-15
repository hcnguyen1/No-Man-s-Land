using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CraftingBench : MonoBehaviour
{
    [Tooltip("Assign the crafting UI GameObject (CraftingMenu panel)")]
    public GameObject craftingUI;

    private bool playerNearby = false;
    private CraftingSystem craftingSystem;
    private Collider2D benchCollider;

    void Awake()
    {
        craftingSystem = new CraftingSystem();
        benchCollider = GetComponent<Collider2D>();
        if (benchCollider != null)
            benchCollider.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            if (craftingUI != null)
                craftingUI.SetActive(false);
        }
    }

    void Update()
    {
        // Removed E key handling - crafting menu is now opened with K key via TabManager
        // This allows the inventory (E key) to work properly when near the bench
    }

    public CraftingSystem GetCraftingSystem()
    {
        return craftingSystem;
    }

    public bool IsPlayerNearby()
    {
        return playerNearby;
    }
}
