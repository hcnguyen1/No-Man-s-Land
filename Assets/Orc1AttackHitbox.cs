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
        Entity player = collision.GetComponent<Entity>();
        if (player != null)
        {
            player.TakeDamage(orc.AttackPower);
        }
    }
}
