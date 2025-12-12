using UnityEngine;
using TMPro;

public class CurrencyUI : MonoBehaviour
{
    [SerializeField]
    private TMP_Text currencyText; // The text component to display currency

    [SerializeField]
    private Player player; // Reference to the player

    private int lastCurrency = -1; // Track last currency to only update when changed

    [SerializeField]
    private bool persistAcrossScenes = true; // Option to make UI persist across scenes

    private void Awake()
    {
        if (persistAcrossScenes)
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        if (player == null)
        {
            player = FindObjectOfType<Player>();
            if (player == null)
            {
                Debug.LogError("CurrencyUI: No Player found in scene!");
            }
        }

        UpdateCurrencyDisplay();
    }

    private void Update()
    {
        // Only update if currency changed (efficient)
        if (player != null && player.currency != lastCurrency)
        {
            UpdateCurrencyDisplay();
        }
    }

    private void UpdateCurrencyDisplay()
    {
        if (player != null && currencyText != null)
        {
            lastCurrency = player.currency;
            currencyText.text = $"Gold: {player.currency}";
        }
    }
}
