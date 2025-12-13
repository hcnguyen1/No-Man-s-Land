using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Orc1 : Entity
{
    private Transform playerTransform;
    private Rigidbody2D rb;
    private Animator animator;

    // Variable to collect all the child hitboxes
    private List<Collider2D> hitboxes;

    // Track if Orc is attacking to prevent spam
    private bool isAttacking = false;
    private bool canAttack = true;

    // Track death state
    private bool noHealth;

    [SerializeField] private AudioClip attackSFX;
    [SerializeField] private GameObject goldCoinPrefab; // The coin will now drop on death

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
    }

    private void FixedUpdate()
    {
        if (playerTransform == null) return;
        
        // Attack if in range
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer <= attackRange && canAttack && !noHealth)
        {
            StartAttackWindow();
        }

        // Chase Player
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
            
            animator.SetBool("isWalking", !isAttacking);
        }
        else
        {
            rb.velocity = Vector2.zero;
            animator.SetBool("isWalking", false);
            return;
        }
    }

    // Orc Attack
    // Turn on isAttacking when animation starts, turn off when animation ends
    public void StartAttackWindow()
    {
        if(isAttacking) return;
        isAttacking = true;
        canAttack = false;
        animator.SetBool("isAttacking", true);
        PlayAttackSFX();
    }
    public void EndAttackWindow()
    {
        // Start cooldown before next attack
        StartCoroutine(AttackCooldown());
        isAttacking = false;
        animator.SetBool("isAttacking", false);
    }
    
    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    // Orc Death (Animation handles Die() after animation completes)
    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        // Death handling moved to OnZeroHealth()
    }

    // Orc Attack Range Visualization
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    private void PlayAttackSFX()
    {
        if (audioSource != null && attackSFX != null)
        {
            audioSource.PlayOneShot(attackSFX);
        }
    }

    protected override void OnZeroHealth()
    {
        if (!noHealth)
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
        // Do not destroy here; animation should invoke Die() via event
    }

    protected override void Die()
    {
        // Spawn gold coin on death
        if (goldCoinPrefab != null)
        {
            Instantiate(goldCoinPrefab, transform.position, Quaternion.identity);
        }

        // Call base Die() to handle death sound and destruction
        base.Die();
    }
}
