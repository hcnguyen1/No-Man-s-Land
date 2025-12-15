using System.Collections;
using UnityEngine;

public class CurrencyItem : MonoBehaviour
{
    [SerializeField]
    private int currencyValue = 100; // How much currency this item gives
    
    [SerializeField]
    private AudioClip pickupSound; // Sound to play when picked up

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player != null)
        {
            player.currency += currencyValue;
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
        // Play pickup sound
        if (pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
        }
        
        Destroy(gameObject);
        yield return null;
    }
}
