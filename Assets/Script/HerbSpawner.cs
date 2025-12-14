using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inventory.Model;

public class HerbSpawner : MonoBehaviour
{
    [SerializeField] private ItemSO herbItemSO; // The herb ItemSO to spawn
    [SerializeField] private int maxHerbAmount = 15; // Maximum herbs allowed at once
    [SerializeField] private int spawnInterval = 10; // Seconds between herb spawns
    [SerializeField] private float spawnRadius = 20f; // Radius around this spawner to place herbs

    private void Start()
    {
        StartCoroutine(SpawnHerbWithInterval());
    }

    private void SpawnHerb()
    {
        // Count current herbs in scene
        int currentHerbCount = GameObject.FindGameObjectsWithTag("Herb").Length;

        // Only spawn if under max amount
        if (currentHerbCount < maxHerbAmount && herbItemSO != null)
        {
            // Random position within radius of spawner
            Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
            Vector2 spawnPosition = (Vector2)transform.position + randomOffset;

            // Create the herb GameObject
            GameObject herb = new GameObject("Herb");
            herb.tag = "Herb"; // Tag it so we can count them
            herb.transform.position = spawnPosition;
            herb.transform.localScale = new Vector3(0.5f, 0.5f, 1f); // Half size

            // Add sprite renderer
            SpriteRenderer sr = herb.AddComponent<SpriteRenderer>();
            sr.sprite = herbItemSO.ItemImage;
            sr.sortingLayerName = "Foreground"; // Adjust if needed

            // Add collider for pickup
            CircleCollider2D col = herb.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = 0.5f;

            // Add Item script so player can pick it up
            Item itemScript = herb.AddComponent<Item>();
            itemScript.Initialize(herbItemSO, 1);

            Debug.Log($"Spawned herb at {spawnPosition}. Current count: {currentHerbCount + 1}");
        }
    }

    private IEnumerator SpawnHerbWithInterval()
    {
        while (true)
        {
            SpawnHerb();
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
