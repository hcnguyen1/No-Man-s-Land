using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth, health;
    [SerializeField] protected int attackPower;
    [SerializeField] protected float movementSpeed;
    [SerializeField] protected float attackCooldown;
    [SerializeField] protected float attackRange;

    protected virtual void Awake()
    {
        Initialize();
    }

    protected virtual void Initialize()
    {
        health = maxHealth;
    }

    public virtual void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            health = 0;
            Die();
        }
    }

    protected virtual void Die()
    {
        // Debug log of that entity's name has died
        Debug.Log($"{gameObject.name} has died.");
        Destroy(gameObject);
    }
}
