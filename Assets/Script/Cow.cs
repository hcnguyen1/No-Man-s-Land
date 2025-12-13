using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inventory.Model;

public class Cow : Animal
{
    private bool noHealth;
    private Collider2D cowCollider;

    [SerializeField] private ItemSO rawMeat; // RawMeat ItemSO to drop on death

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

    protected override void Die()
    {
        // Spawn meat item in world on death
        if (rawMeat != null)
        {
            GameObject droppedMeat = new GameObject("DroppedMeat");
            droppedMeat.transform.position = transform.position;
            
            // Add sprite renderer
            SpriteRenderer sr = droppedMeat.AddComponent<SpriteRenderer>();
            sr.sprite = rawMeat.ItemImage;
            
            // Add collider
            CircleCollider2D col = droppedMeat.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = 0.5f;
            
            // Add Item script and initialize it
            Item itemScript = droppedMeat.AddComponent<Item>();
            itemScript.Initialize(rawMeat, 1);
        }

        // Call base Die() to handle death sound and destruction
        base.Die();
    }
}
