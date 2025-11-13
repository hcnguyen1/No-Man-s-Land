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

    // Mob spawning logic helper
    public int dayCount = 0;
    public int nightCount = 0;

    // Initialize private gameobject list that can add any number of enemy prefabs
    [SerializeField] private List<GameObject> nightOneEnemyPrefabs;

    void Start()
    {
        InitializeLight();
    }

    void Update()
    {
        LogSeconds();
        if (isTransitioning)
            HandleTransition();
        else
            HandleDayNightCycle();
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
            Debug.Log($"Second: {currentSecond}");
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
            Debug.Log($"Day {dayCount} has begun.");
        }
        else
        {
            Debug.Log($"Night {nightCount} has begun.");
        }

        // Flip targets for next cycle
        targetColor = isDay ? nightColor : dayColor;
        targetIntensity = isDay ? nightIntensity : dayIntensity;

        if (!isDay)
        {
            nightCount++;
            NightOne();
            NightTwo();
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

    // Spawns mobs for Night One
    private void NightOne()
    {
        // Spawn 2 Orc1 mobs at random positions inside "Ground" tagged area
        GameObject[] grounds = GameObject.FindGameObjectsWithTag("Ground");
        if (nightCount == 1)
        {
            if (grounds.Length == 0 || nightOneEnemyPrefabs == null || nightOneEnemyPrefabs.Count == 0) return;

            for (int i = 0; i < 1; i++)
            {
                GameObject ground = grounds[Random.Range(0, grounds.Length)];
                Vector3 spawnPos = ground.transform.position + new Vector3(
                    Random.Range(-2f, 2f),
                    Random.Range(-2f, 2f),
                    0f
                );
                // Log
                Debug.Log($"Spawning Night One Enemy at: {spawnPos}");

                Instantiate(nightOneEnemyPrefabs[Random.Range(0, nightOneEnemyPrefabs.Count)], spawnPos, Quaternion.identity);
            }
        }
    }

    // Spawns mobs for Night Two
    private void NightTwo()
    {
        // Spawn 2 mobs at random positions inside "Ground" tagged area
        GameObject[] grounds = GameObject.FindGameObjectsWithTag("Ground");
        if (nightCount == 2)
        {
            if (grounds.Length == 0 || nightOneEnemyPrefabs == null || nightOneEnemyPrefabs.Count == 0) return;
            for (int i = 0; i < 2; i++)
            {
                GameObject ground = grounds[Random.Range(0, grounds.Length)];
                Vector3 spawnPos = ground.transform.position + new Vector3(
                    Random.Range(-2f, 2f),
                    Random.Range(-2f, 2f),
                    0f
                );
                // Log
                Debug.Log($"Spawning Night Two Enemy at: {spawnPos}");

                Instantiate(nightOneEnemyPrefabs[Random.Range(0, nightOneEnemyPrefabs.Count)], spawnPos, Quaternion.identity);
            }
        }
    }
}
