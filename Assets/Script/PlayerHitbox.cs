using UnityEngine;

public class PlayerHitbox : MonoBehaviour
{
    [SerializeField] private int damage = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Entity enemy = collision.GetComponent<Entity>();
        if (enemy != null && enemy != GetComponentInParent<Player>())
        {
            enemy.TakeDamage(damage);
            // Optionally, disable hitbox after hit to prevent multi-hit
            // gameObject.SetActive(false);
        }
    }
}
