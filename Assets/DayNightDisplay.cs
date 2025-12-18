using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class DayNightDisplay : MonoBehaviour
{
    // Find DayNightManager in the scene
    [SerializeField] DayNightManager dayNightManager;
    [SerializeField] TMP_Text textComponent;

    private void Start()
    {
        textComponent = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        updateTextUI();
    }

    private void updateTextUI()
    {
        if (dayNightManager != null && textComponent != null)
        {
            if(dayNightManager.isDay)
            {
                textComponent.text = "Day " + dayNightManager.dayCount.ToString();
            }
            else
            {
                textComponent.text = "Night " + dayNightManager.dayCount.ToString();
            }
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reassign DayNightManager reference when a new scene is loaded
        dayNightManager = FindObjectOfType<DayNightManager>();
        updateTextUI();
    }
}
