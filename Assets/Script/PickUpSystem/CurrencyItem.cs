using System.Collections;
using UnityEngine;

public class CurrencyItem : MonoBehaviour
{
    [SerializeField]
    private int currencyValue = 100; // How much currency this item gives

    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private float duration = 0.3f;

    [SerializeField]
    private Sprite currencySprite; // Optional: set a sprite for the currency

    private void Start()
    {
        if (currencySprite != null)
        {
            GetComponent<SpriteRenderer>().sprite = currencySprite;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player != null)
        {
            player.currency += currencyValue;
            Debug.Log($"Picked up {currencyValue} currency! Total: {player.currency}");
            DestroyCurrency();
        }
    }

    private void DestroyCurrency()
    {
        GetComponent<Collider2D>().enabled = false;
        StartCoroutine(AnimateCurrencyPickup());
    }

    private IEnumerator AnimateCurrencyPickup()
    {
        if (audioSource != null)
        {
            audioSource.Play();
        }

        Vector3 startScale = transform.localScale;
        Vector3 endScale = Vector3.zero;
        float currentTime = 0;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScale, endScale, currentTime / duration);
            yield return null;
        }

        Destroy(gameObject);
    }
}
