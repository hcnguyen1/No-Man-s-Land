using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth;
    public float health;
    [SerializeField] protected int attackPower;
    [SerializeField] protected float movementSpeed;
    [SerializeField] protected float attackCooldown;
    [SerializeField] protected float attackRange;

    [Header("Audio")]
    [SerializeField] protected AudioClip getHitSFX;
    
    private AudioSource _audioSource;
    protected AudioSource audioSource
    {
        get
        {
            if (_audioSource == null)
                _audioSource = GetComponent<AudioSource>();
            return _audioSource;
        }
    }

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
        PlayGetHitSFX();
    }

    protected virtual void PlayGetHitSFX()
    {
        if (audioSource != null && getHitSFX != null)
        {
            audioSource.PlayOneShot(getHitSFX);
        }
    }

    protected virtual void Die()
    {
        // Debug log of that entity's name has died
        if (health <= 0)
        {
            Debug.Log($"{gameObject.name} has died.");
            Destroy(gameObject);
        }
    }

    // Getter for attackPower
    public int AttackPower => attackPower;
}
