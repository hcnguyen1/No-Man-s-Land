using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightController : MonoBehaviour
{
    public Light2D light2D;

    // day: #FFF3C2, Intensity: 1.0
    // night: #4A5DAA, Intensity: 0.25
    public Color dayColor = new Color(1f, 0.9529412f, 0.7607843f);
    public Color nightColor = new Color(0.2901961f, 0.3607843f, 0.6666667f);
    public float dayIntensity = 1.0f;
    public float nightIntensity = 0.25f;

    public float dayDuration = 15f;
    public float nightDuration = 15f;

    private bool isDay = true;
    private float timer = 0f;
    private float transitionDuration = 5f;
    private float transitionTimer = 0f;
    private bool isTransitioning = false;

    private Color targetColor;
    private float targetIntensity;
    private Color initialColor;
    private float initialIntensity;
    private int lastLoggedSecond = -1;

    private MerchantNPC merchantNPC;

    // Mob spawning logic helper
    public int dayCount = 1;
    public int nightCount = 0;
    [SerializeField] float mobSpawnDelay = 3f;

    [SerializeField] float animalSpawnChance = 0.2f;
    [SerializeField] float animalSpawnTimer = 0f;
    [SerializeField] float animalSpawnInterval = 1f; // Seconds
    [SerializeField] int initialAnimalSpawnCount = 5;
    // Initialize private gameobject list of mobs
    [SerializeField] private List<GameObject> animalPrefabs;
    [SerializeField] private List<GameObject> nightOneEnemyPrefabs;

    [Header("Night 1")]
    [SerializeField] int nightOneEnemiesPerSpawn = 1;
    [SerializeField] int nightOneSpawnAmount = 5;

    [Header("Night 2")]
    [SerializeField] int nightTwoEnemiesPerSpawn = 1;
    [SerializeField] int nightTwoSpawnAmount = 5;

    [Header("Night 3")]
    [SerializeField] int nightThreeEnemiesPerSpawn = 1;
    [SerializeField] int nightThreeSpawnAmount = 5;

    [Header("Night 4")]
    [SerializeField] int nightFourEnemiesPerSpawn = 1;
    [SerializeField] int nightFourSpawnAmount = 5;

    [Header("Night 5")]
    [SerializeField] int nightFiveEnemiesPerSpawn = 1;
    [SerializeField] int nightFiveSpawnAmount = 5;

    [Header("Night 6")]
    [SerializeField] int nightSixEnemiesPerSpawn = 1;
    [SerializeField] int nightSixSpawnAmount = 5;

    void Start()
    {
        InitializeLight();
        merchantNPC = FindObjectOfType<MerchantNPC>(true);

        SpawnInitialAnimals();
    }

    void Update()
    {
        LogSeconds();
        if (isTransitioning)
            HandleTransition();
        else
            HandleDayNightCycle();

        // Spawn animals by chance during the day on ground tiles
        if (isDay)
        {
            animalSpawnTimer += Time.deltaTime;
            if (animalSpawnTimer >= animalSpawnInterval)
            {
                SpawnAnimalsByChance(animalSpawnChance, animalPrefabs);
                animalSpawnTimer = 0f;
            }
        }

        ToggleMerchantNPC();
    }

    // Initializes the light to day settings
    private void InitializeLight()
    {
        light2D.color = dayColor;
        light2D.intensity = dayIntensity;
        targetColor = nightColor;
        targetIntensity = nightIntensity;
    }

    // Logs the current second to the console
    private void LogSeconds()
    {
        int currentSecond = Mathf.FloorToInt(Time.time);
        if (currentSecond != lastLoggedSecond)
        {
            lastLoggedSecond = currentSecond;
        }
    }

    // Handles the transition between day and night
    private void HandleTransition()
    {
        transitionTimer += Time.deltaTime;
        float t = transitionTimer / transitionDuration;
        light2D.color = Color.Lerp(initialColor, targetColor, t);
        light2D.intensity = Mathf.Lerp(initialIntensity, targetIntensity, t);

        if (transitionTimer >= transitionDuration)
        {
            FinishTransition();
        }
    }

    private void FinishTransition()
    {
        isTransitioning = false;
        isDay = !isDay;
        timer = 0f;

        // Log if it's now day or night and what day or night it is.
        if (isDay)
        {
            dayCount++;
        }
        else
        {
            nightCount++;
        }

        // Flip targets for next cycle
        targetColor = isDay ? nightColor : dayColor;
        targetIntensity = isDay ? nightIntensity : dayIntensity;

        // Spawn mobs when it's night
        if (!isDay)
        {
            nightCount++;
            switch (nightCount)
            {
                case 1:
                    NightOne();
                    break;
                case 2:
                    NightTwo();
                    break;
                case 3:
                    NightThree();
                    break;
                case 4:
                    NightFour();
                    break;
                case 5:
                    NightFive();
                    break;
                case 6:
                    NightSix();
                    break;
            }
        }
    }

    // Manages the day-night cycle timing
    private void HandleDayNightCycle()
    {
        timer += Time.deltaTime;
        float duration = isDay ? dayDuration : nightDuration;

        if (timer >= duration)
        {
            StartTransition();
        }
    }

    // Starts the transition process between day and night
    private void StartTransition()
    {
        isTransitioning = true;
        transitionTimer = 0f;
        initialColor = light2D.color;
        initialIntensity = light2D.intensity;
    }

    private void ToggleMerchantNPC()
    {
        if (merchantNPC != null)
        {
            merchantNPC.gameObject.SetActive(isDay);
        }
    }
    private void SpawnInitialAnimals()
    {
        for (int i = 0; i < initialAnimalSpawnCount; i++)
        {
            SpawnAnimals(animalPrefabs);
        }
    }

    private void SpawnAnimals(List<GameObject> animalPrefabs)
    {
        // Search for all tilemap object tagged with "Ground"
        GameObject[] groundTiles = GameObject.FindGameObjectsWithTag("Ground");

        GameObject randomTile = groundTiles[Random.Range(0, groundTiles.Length)]; // ranges from the list of "Ground" tiles
        Collider2D tilemapCollider = randomTile.GetComponent<Collider2D>(); // We use collider to get the bounds of the tilemap, and the bounds means the area of the tilemap

        if (tilemapCollider != null)
        {
            Bounds bounds = tilemapCollider.bounds;
            Vector2 randomPosition = new Vector2(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y)
            );

            // Spawn animal at the randomly generated position
            GameObject randomAnimal = animalPrefabs[Random.Range(0, animalPrefabs.Count)];
            Instantiate(randomAnimal, randomPosition, Quaternion.identity);
        }
    }
    // Simple spawn animals by chance during the day on ground tiles
    private void SpawnAnimalsByChance(float chance, List<GameObject> animalPrefabs)
    {
        if (Random.value < chance)
        {        
            // Search for all tilemap object tagged with "Ground"
            GameObject[] groundTiles = GameObject.FindGameObjectsWithTag("Ground");

            GameObject randomTile = groundTiles[Random.Range(0, groundTiles.Length)]; // ranges from the list of "Ground" tiles
            Collider2D tilemapCollider = randomTile.GetComponent<Collider2D>(); // We use collider to get the bounds of the tilemap, and the bounds means the area of the tilemap

            if (tilemapCollider != null)
            {
                Bounds bounds = tilemapCollider.bounds;
                Vector2 randomPosition = new Vector2(
                    Random.Range(bounds.min.x, bounds.max.x),
                    Random.Range(bounds.min.y, bounds.max.y)
                );

                // Spawn animal at the randomly generated position
                GameObject randomAnimal = animalPrefabs[Random.Range(0, animalPrefabs.Count)];
                Instantiate(randomAnimal, randomPosition, Quaternion.identity);
            }
        }
    }

    IEnumerator SpawnEnemiesWithDelay(List<GameObject> enemyPrefabs, float delay, int enemiesPerSpawn, int totalSpawns)
    {
        // Search for all tilemap object tagged with "Ground"
        GameObject[] groundTiles = GameObject.FindGameObjectsWithTag("Ground");

        for (int spawn = 0; spawn < totalSpawns; spawn++)
        {
            for (int enemy = 0; enemy < enemiesPerSpawn; enemy++)
            {
                // Pick random enemy from the list of enemy prefabs
                GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];

                // Pick random tile from "Ground" tagged tiles
                GameObject randomTile = groundTiles[Random.Range(0, groundTiles.Length)]; // ranges from the list of "Ground" tiles
                Collider2D tilemapCollider = randomTile.GetComponent<Collider2D>(); // We use collider to get the bounds of the tilemap, and the bounds means the area of the tilemap

                if (tilemapCollider != null)
                {
                    Bounds bounds = tilemapCollider.bounds;
                    Vector2 randomPosition = new Vector2(
                        Random.Range(bounds.min.x, bounds.max.x),
                        Random.Range(bounds.min.y, bounds.max.y)
                    );

                    // Spawn enemy at the randomly generated position
                    Instantiate(enemyPrefab, randomPosition, Quaternion.identity);
                }
            }
            // Spawning delay
            if (spawn < totalSpawns - 1)
            {
                yield return new WaitForSeconds(delay);
            }
        }
    }

    // Spawns mobs for Night One
    private void NightOne()
    {
        if (nightCount == 1)
        {
            StartCoroutine(SpawnEnemiesWithDelay(nightOneEnemyPrefabs, mobSpawnDelay, nightOneEnemiesPerSpawn, nightOneSpawnAmount));
        }
    }

    // Spawns mobs for Night Two
    private void NightTwo()
    {
        if (nightCount == 2)
        {
            StartCoroutine(SpawnEnemiesWithDelay(nightOneEnemyPrefabs, mobSpawnDelay, nightTwoEnemiesPerSpawn, nightTwoSpawnAmount));
        }
    }
    private void NightThree()
    {
        if (nightCount == 3)
        {
            StartCoroutine(SpawnEnemiesWithDelay(nightOneEnemyPrefabs, mobSpawnDelay, nightThreeEnemiesPerSpawn, nightThreeSpawnAmount));
        }
    }
    private void NightFour()
    {
        if (nightCount == 4)
        {
            StartCoroutine(SpawnEnemiesWithDelay(nightOneEnemyPrefabs, mobSpawnDelay, nightFourEnemiesPerSpawn, nightFourSpawnAmount));
        }
    }
    private void NightFive()
    {
        if (nightCount == 5)
        {
            StartCoroutine(SpawnEnemiesWithDelay(nightOneEnemyPrefabs, mobSpawnDelay, nightFiveEnemiesPerSpawn, nightFiveSpawnAmount));
        }
    }

    // Night 6++++++++++++++++++++
    private void NightSix()
    {
        if (nightCount == 6)
        {
            // Kill the player by making the game 100x harder
            // StartCoroutine(SpawnEnemiesWithDelay(nightOneEnemyPrefabs, mobSpawnDelay, nightSixEnemiesPerSpawn, nightSixSpawnAmount));
        }
    }
}
