using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orc1AttackHitbox : MonoBehaviour
{
    private Orc1 orc;

    private void Awake()
    {
        orc = GetComponentInParent<Orc1>();
        if (orc == null)
        {
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Tree") || collision.CompareTag("Animal") || collision.CompareTag("Stone"))
        {
            Entity target = collision.GetComponent<Entity>();
            if (target != null)
            {
                target.TakeDamage(orc.AttackPower);
            }
        }
    }
}
