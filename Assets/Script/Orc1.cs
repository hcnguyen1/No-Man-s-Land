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

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        
        if (player != null)
        {
            playerTransform = player.transform;
        } else
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
        ChasePlayer();

        // If Orc is within attack range of player 
        if (Vector2.Distance(transform.position, playerTransform.position) <= attackRange)
        {
            StartCoroutine(Attack());
        }
    }
    
    private void ChasePlayer()
    {
        Vector2 pos = rb.position;
        Vector2 target = playerTransform.position;
        Vector2 dir = (target - pos).normalized;

        rb.MovePosition(pos + dir * movementSpeed * Time.fixedDeltaTime);

        // Update animator parameters
        animator.SetFloat("MoveX", dir.x);
        animator.SetFloat("MoveY", dir.y);
        animator.SetBool("isWalking", true);
    }

    // Orc Attack
    private IEnumerator Attack()
    {
        animator.SetBool("isAttacking", true);
        yield return new WaitForSeconds(attackCooldown);
        animator.SetBool("isAttacking", false);
    }

    // When hitbox collides with player
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            var player = collision.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(attackPower);
            }
        }
    }

    // Orc Death
    private void Die()
    {
        Debug.Log("Orc1 dies.");
    }

    // Orc Attack Range Visualization
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
