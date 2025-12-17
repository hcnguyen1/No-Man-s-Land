using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSpawner : MonoBehaviour
{
    [SerializeField] GameObject[] treePrefabs;
    [SerializeField] int maxTreeAmount = 25;
    [SerializeField] int spawnInterval = 5;

    private void Start()
    {
        SpawnInitialTrees();
        StartCoroutine(SpawnTreePerSecond());
    }

    private void SpawnInitialTrees()
    {
        for (int i = 0; i < maxTreeAmount; i++)
        {
            SpawnTree();
        }
    }

    private void SpawnTree()
    {
        // Grab all "Ground" tagged tiles in the scene
        GameObject[] groundTiles = GameObject.FindGameObjectsWithTag("Ground");

        // Select a random ground tile
        GameObject randomTile = groundTiles[Random.Range(0, groundTiles.Length)];

        Collider2D tilemapCollider = randomTile.GetComponent<Collider2D>(); // Get the collider of the tilemap

        int currentTreeCount = GameObject.FindGameObjectsWithTag("Tree").Length;

        if (tilemapCollider != null && currentTreeCount < maxTreeAmount)
        {

            // Get a random position within the bounds of the tile's collider
            Bounds tileBounds = tilemapCollider.bounds;
            float randomX = Random.Range(tileBounds.min.x, tileBounds.max.x);
            float randomY = Random.Range(tileBounds.min.y, tileBounds.max.y);
            Vector2 spawnPosition = new Vector2(randomX, randomY);

            // Spawn offset to prevent spawning at exact center of tile
            Vector2 spawnOffset = new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));
            Vector2 finalSpawnPosition = spawnPosition + spawnOffset;

            // Instantiate a random tree prefab at the calculated position
            GameObject randomTreePrefab = treePrefabs[Random.Range(0, treePrefabs.Length)];
            Instantiate(randomTreePrefab, finalSpawnPosition, Quaternion.identity);

            currentTreeCount = GameObject.FindGameObjectsWithTag("Tree").Length;

            Debug.Log("Spawned tree at: " + finalSpawnPosition);
            Debug.Log("Current tree count: " + currentTreeCount);
        }


    }
    IEnumerator SpawnTreePerSecond()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            int currentTreeCount = GameObject.FindGameObjectsWithTag("Tree").Length;
            if (currentTreeCount < maxTreeAmount)
            {
                SpawnTree();
            }
        }
    }
}
