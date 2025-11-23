using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orc1 : Entity
{
    private Transform playerTransform;
    private Rigidbody2D rb;
    private Animator animator;

    // Variable to collect all the child hitboxes
    private List<Collider2D> hitboxes;

    // Track if Orc is attacking to prevent spam
    private bool isAttacking;
    private bool hasDealtDamage;
    private float lastAttackTime = -Mathf.Infinity; // Track time of last attack
    private float attackAnimationDuration = 0.26f; // Duration of attack animation

    // Track death state
    private bool noHealth;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Collider2D mainCollider = GetComponent<Collider2D>();

        // Start with all hitboxes disabled
        hitboxes = new List<Collider2D>();
        foreach (Collider2D hitbox in GetComponentsInChildren<Collider2D>())
        {
            // Only disable child hitboxes, not the main collider
            if (hitbox != mainCollider)
            {
                hitbox.enabled = false;
                hitboxes.Add(hitbox);
            }
        }

        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("Player object not found in the scene.");
        }
    }

    private void FixedUpdate()
    {
        // Chases the player
        if (playerTransform == null)
        {
            return;
        }
        // If Orc is within attack range of player 
        if (Vector2.Distance(transform.position, playerTransform.position) <= attackRange)
        {
            if (!isAttacking && Time.time >= lastAttackTime + attackCooldown)
            {
                StartCoroutine(Attack());
            }
        }
        ChasePlayer();
    }

    private void ChasePlayer()
    {
        Vector2 pos = rb.position;
        Vector2 target = playerTransform.position;
        Vector2 dir = (target - pos).normalized;

        // Move only if Orc is alive
        if (!noHealth)
        {
            rb.MovePosition(pos + dir * movementSpeed * Time.fixedDeltaTime);

            animator.SetFloat("MoveX", dir.x);
            animator.SetFloat("MoveY", dir.y);
            animator.SetBool("isWalking", true);

        }
        else
        {
            rb.velocity = Vector2.zero;
            animator.SetBool("isWalking", false);
            return;
        }
    }

    // Orc Attack
    private IEnumerator Attack()
    {
        isAttacking = true;
        hasDealtDamage = false; // Have not dealt damage yet this attack
        animator.SetBool("isAttacking", true);

        yield return new WaitForSeconds(attackAnimationDuration); // Duration of attack animation

        animator.SetBool("isAttacking", false);
        lastAttackTime = Time.time; // This records the time of the last attack
        isAttacking = false;
    }

    // When hitbox collides with player
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && isAttacking && !hasDealtDamage)
        {
            Entity player = collision.GetComponent<Entity>();
            if (player != null)
            {
                player.TakeDamage(attackPower);
                hasDealtDamage = true; // Ensure damage is only dealt once per attack
                Debug.Log("Orc has dealt damage to Player.");
            }
        }
    }

    // Orc Death (Animation handles Die() after animation completes)
    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);

        if (health <= 0 && !noHealth)
        {
            noHealth = true;
            animator.SetBool("noHealth", true);

            // Disable all hitboxes upon death
            foreach (Collider2D hitbox in hitboxes)
            {
                hitbox.enabled = false;
            }

            // Disable main collider to prevent further interactions
            Collider2D mainCollider = GetComponent<Collider2D>();
            if (mainCollider != null)
            {
                mainCollider.enabled = false;
            }
        }
    }

    // Orc Attack Range Visualization
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }



}
