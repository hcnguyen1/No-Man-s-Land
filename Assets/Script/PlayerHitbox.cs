using UnityEngine;

public class PlayerHitbox : MonoBehaviour
{
    private Player player;

    // This method is called when the script instance is being loaded
    private void Awake()
    {
        player = GetComponentInParent<Player>();
        if (player == null)
        {
            Debug.LogError("Player component not found in parent.");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Entity enemy = collision.GetComponent<Entity>();
        if (enemy != null)
        {
            enemy.TakeDamage(player.AttackPower);
        }
    }
}
