using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cow : Animal
{
    private bool noHealth;
    private Collider2D cowCollider;

    void Start()
    {
        cowCollider = GetComponent<Collider2D>();
        noHealth = false;
    }

    void Update()
    {
        Wander();
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        animator.SetBool("takenDamage", true);
        if (health <= 0 && !noHealth)
        {
            noHealth = true;
            animator.SetTrigger("noHealth");
        }

        if (noHealth && cowCollider != null)
        {
            cowCollider.enabled = false;
        }
    }

    public void EndDamageAnimation()
    {
        animator.SetBool("takenDamage", false);
    }
}
