using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightController : MonoBehaviour
{
    public Light2D light2D;
    // Timer moving between day and night is 15 seconds

    // day: #FFF3C2, Intensity: 1.0
    // night: #4A5DAA, Intensity: 0.25

    // The color changes gradually rather than abruptly
    public Color dayColor = new Color(1f, 0.9529412f, 0.7607843f); // #FFF3C2
    public Color nightColor = new Color(0.2901961f, 0.3607843f, 0.6666667f); // #4A5DAA
    public float dayIntensity = 1.0f;
    public float nightIntensity = 0.25f;

    public float dayDuration = 15f;
    public float nightDuration = 15f;

    private bool isDay = true;
    private float timer = 0f;
    private float transitionDuration = 5f; // Duration of the transition between day and night
    private float transitionTimer = 0f;
    private bool isTransitioning = false;
    private Color targetColor;
    private float targetIntensity;
    private Color initialColor;
    private float initialIntensity;
    private int lastLoggedSecond = -1;

    void Start()
    {
        light2D.color = dayColor;
        light2D.intensity = dayIntensity;
        targetColor = nightColor;
        targetIntensity = nightIntensity;
    }

    void Update()
    {
        int currentSecond = Mathf.FloorToInt(Time.time);
        if(currentSecond != lastLoggedSecond)
        {
            Debug.Log($"Second: {currentSecond}");
            lastLoggedSecond = currentSecond;
        }


        if (isTransitioning)
        {
            transitionTimer += Time.deltaTime;
            float t = transitionTimer / transitionDuration;
            light2D.color = Color.Lerp(initialColor, targetColor, t);
            light2D.intensity = Mathf.Lerp(initialIntensity, targetIntensity, t); 

            if (transitionTimer >= transitionDuration)
            {
                isTransitioning = false;
                isDay = !isDay;
                timer = 0f;
                targetColor = isDay ? nightColor : dayColor;
                targetIntensity = isDay ? nightIntensity : dayIntensity;
            }
        }
        else
        {
            timer += Time.deltaTime; // Increment the timer that tracks day/night duration
            float duration = isDay ? dayDuration : nightDuration;
            if (timer >= duration)
            {
                isTransitioning = true;
                transitionTimer = 0f;
                initialColor = light2D.color;
                initialIntensity = light2D.intensity;
            }
        }
    }
}