using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerUIManager : MonoBehaviour
{
    public Image healthBar, hungerBar, thirstBar;
    public float healthAmount, hungerAmount, thirstAmount;

    // Connect to Player script to get health, hunger, thirst values rather than hardcoding
    [SerializeField] Player player;

    // UI Variables
    [SerializeField] GameObject PauseMenuUI;
    [SerializeField] GameObject GameOverUI;
    [SerializeField] GameObject VictoryUI;
    private bool isPaused = false;
    private bool isGameOver = false;
    private bool displayVictoryOnce = false;

    void Start()
    {
        healthAmount = player.health;
        hungerAmount = player.hunger;
        thirstAmount = player.thirst;

        if (PauseMenuUI) PauseMenuUI.SetActive(false);
        if (GameOverUI) GameOverUI.SetActive(false);
        if (VictoryUI) VictoryUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // Pause Menu and Game Over Menu Toggles
        if (player == null || player.health <= 0 && !isGameOver)
        {
            ToggleGameOverMenu();
            return;
        }
        if (Input.GetKeyDown(KeyCode.Escape) && !isGameOver)
        {
            TogglePauseMenu();
        }
        if (isPaused || isGameOver)
        {
            return;
        }

        // In-game UI Updates
        if (player == null)
        {
            SetBarToZero(); // Edge case: if player is dead, set all bars to zero
            return;
        }
        UpdateHealthBar();
        UpdateHungerBar();
        UpdateThirstBar();

        // Day 6 (You get this from DayNightManager), trigger victory UI once
        // if in scene Level1, Level2, or Level3, find DayNightManager
        if (SceneManager.GetActiveScene().name.Contains("Level"))
        {
            DayNightManager dayNightManager = FindObjectOfType<DayNightManager>();
            if (dayNightManager != null)
            {
                if (dayNightManager.dayCount >= 6 && !displayVictoryOnce)
                {
                    TriggerVictoryUI();
                }
            }
        }
    }

    private void UpdateHealthBar()
    {
        if (player && healthBar)
            healthBar.fillAmount = Mathf.Clamp01(player.health / player.maxHealth);
    }

    private void UpdateHungerBar()
    {
        if (player && hungerBar)
            hungerBar.fillAmount = Mathf.Clamp01(player.hunger / player.maxHunger);
    }

    private void UpdateThirstBar()
    {
        if (player && thirstBar)
            thirstBar.fillAmount = Mathf.Clamp01(player.thirst / player.maxThirst);
    }

    private void SetBarToZero()
    {
        if (healthBar) healthBar.fillAmount = 0f;
        if (hungerBar) hungerBar.fillAmount = 0f;
        if (thirstBar) thirstBar.fillAmount = 0f;
    }

    // Resume, Pause, and Game Over Menu Functions
    private void TogglePauseMenu()
    {
        isPaused = !isPaused;

        if (PauseMenuUI)
        {
            PauseMenuUI.SetActive(isPaused);
        }

        Time.timeScale = isPaused ? 0f : 1f;
    }

    private void ToggleGameOverMenu()
    {
        isGameOver = true;

        if (GameOverUI)
        {
            GameOverUI.SetActive(true);
        }

        SetBarToZero();
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        if (isPaused)
        {
            TogglePauseMenu();
        }
    }

    public void ExitToMainMenu()
    {
        // Set time scale back to normal for main menu
        Time.timeScale = 1f;

        // Destroy the rest of the DontDestroyOnLoad objects except the main menu manager
        foreach (GameObject obj in GameObject.FindObjectsOfType<GameObject>())
        {
            if (obj.scene.name == null || obj.scene.name == "DontDestroyOnLoad")
            {
                Destroy(obj);
            }
        }

        // Load Main Menu scene
        SceneManager.LoadScene("MainMenu");
    }

    public void ContinueButton()
    {
        // Set time scale back to normal
        Time.timeScale = 1f;

        VictoryUI.SetActive(false);
    }

    public void TriggerVictoryUI()
    {
        if (VictoryUI)
        {
            displayVictoryOnce = true;
            VictoryUI.SetActive(true);
        }
        Time.timeScale = 0f;
    }



}