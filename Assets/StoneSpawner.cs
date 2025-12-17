using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneSpawner : MonoBehaviour
{
    [SerializeField] GameObject[] stonePrefabs;
    [SerializeField] int maxStoneAmount = 25;
    [SerializeField] int spawnInterval = 5; // 5 seconds

    private void Start()
    {
        SpawnInitialStones();
        StartCoroutine(SpawnStonePerSecond());
    }

    private void SpawnInitialStones()
    {
        for (int i = 0; i < maxStoneAmount; i++)
        {
            SpawnStone();
        }
    }

    private void SpawnStone()
    {
        // Grab all "Ground" tagged tiles in the scene
        GameObject[] groundTiles = GameObject.FindGameObjectsWithTag("Ground");

        // Select a random ground tile
        GameObject randomTile = groundTiles[Random.Range(0, groundTiles.Length)];

        Collider2D tilemapCollider = randomTile.GetComponent<Collider2D>(); // Get the collider of the tilemap

        int currentStoneCount = GameObject.FindGameObjectsWithTag("Stone").Length;

        if (tilemapCollider != null && currentStoneCount < maxStoneAmount)
        {

            // Get a random position within the bounds of the tile's collider
            Bounds tileBounds = tilemapCollider.bounds;
            float randomX = Random.Range(tileBounds.min.x, tileBounds.max.x);
            float randomY = Random.Range(tileBounds.min.y, tileBounds.max.y);
            Vector2 spawnPosition = new Vector2(randomX, randomY);

            // Spawn offset to prevent spawning at exact center of tile
            Vector2 spawnOffset = new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));
            Vector2 finalSpawnPosition = spawnPosition + spawnOffset;

            // Instantiate a random stone prefab at the calculated position
            GameObject randomStonePrefab = stonePrefabs[Random.Range(0, stonePrefabs.Length)];
            Instantiate(randomStonePrefab, finalSpawnPosition, Quaternion.identity);
            currentStoneCount = GameObject.FindGameObjectsWithTag("Stone").Length;

            Debug.Log("Spawned stone at: " + finalSpawnPosition);
            Debug.Log("Current stone count: " + currentStoneCount);
        }


    }
    IEnumerator SpawnStonePerSecond()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            int currentStoneCount = GameObject.FindGameObjectsWithTag("Stone").Length;
            if (currentStoneCount < maxStoneAmount)
            {
                SpawnStone();
            }
        }
    }
}
