using System.Collections;
using UnityEngine;

// For this class, we want the mobs to drop coins and it was better off becoming a prefab that can affect the 
// characters currency value. It becomes an serializefield in player, while the 100 in this class doesn't really matter,
// it is in charge of the destruction of the prefab and its audio source. 
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
