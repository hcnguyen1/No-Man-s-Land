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
        if (!playerNearby) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (craftingUI != null)
                craftingUI.SetActive(!craftingUI.activeSelf);
        }
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
