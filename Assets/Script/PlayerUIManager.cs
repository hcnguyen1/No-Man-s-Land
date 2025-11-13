using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    public Image healthBar, hungerBar, thirstBar;
    public float healthAmount, hungerAmount, thirstAmount;

    // Connect to Player script to get health, hunger, thirst values rather than hardcoding
    [SerializeField] Player player;

    void Start()
    {
        healthAmount = player.health;
        hungerAmount = player.hunger;
        thirstAmount = player.thirst;
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) {
            SetBarToZero(); // Edge case: if player is dead, set all bars to zero
            return;
        }

        UpdateHealthBar();
        UpdateHungerBar();
        UpdateThirstBar();
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
}