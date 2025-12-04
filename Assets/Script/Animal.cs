using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : Entity
{
    // Wandering parameters
    public float wanderRadius = 5f;
    public float wanderInterval = 3f;
    private float wanderTimer; // Timer to track wandering intervals

    // Movement
    private Vector2 targetPosition;
    private Rigidbody2D rb;
    private Animator animator;

    // Sprite Renderer
    private SpriteRenderer spriteRenderer;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        wanderTimer = wanderInterval; // Start wandering immediately
    }

    protected virtual void Wander()
    {
        wanderTimer += Time.deltaTime;

        // Time to pick a new target position
        if (wanderTimer >= wanderInterval)
        {
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            targetPosition = (Vector2)transform.position + randomDirection * wanderRadius;
            wanderTimer = 0f;
        }

        // Moving to new random position within radius
        Vector2 newPosition = Vector2.MoveTowards(rb.position, targetPosition, movementSpeed * Time.deltaTime);
        rb.MovePosition(newPosition);
        animator.SetBool("isMoving", rb.position != targetPosition);

        // Flip sprite based on movement direction
        if (newPosition.x > rb.position.x)
        {
            spriteRenderer.flipX = true; // Facing right
        }
        else if (newPosition.x < rb.position.x)
        {
            spriteRenderer.flipX = false; // Facing left
        }
    }

    protected virtual void Idle()
    {
        return;
    }
}
