using UnityEngine;

public class PlayerHitbox : Player
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Entity enemy = collision.GetComponent<Entity>();
        if (enemy != null)
        {
            enemy.TakeDamage(attackPower);
        }
    }
}
