using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orc1 : Entity
{
    private Transform playerTransform;
    private Rigidbody2D rb;
    private Animator animator;

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
        if (playerTransform == null)
        {
            return;
        }
        ChasePlayer();
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

    // Orc Attack Animation
    private void Attack()
    {
        Debug.Log("Orc1 attacks with power: " + attackPower);
    }

    // Orc Death Animation
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
