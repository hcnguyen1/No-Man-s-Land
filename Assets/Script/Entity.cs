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
    [SerializeField] [Range(0f, 1f)] protected float getHitVolume = 1f;
    [SerializeField] protected AudioClip deathSFX;
    [SerializeField] [Range(0f, 1f)] protected float deathVolume = 1f;
    
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
        
        if (health <= 0)
        {
            Die();
        }
    }

    protected virtual void PlayGetHitSFX()
    {
        if (audioSource != null && getHitSFX != null)
        {
            audioSource.PlayOneShot(getHitSFX, getHitVolume);
        }
    }

    protected virtual void Die()
    {
        // Debug log of that entity's name has died
        if (health <= 0)
        {
            Debug.Log($"{gameObject.name} has died.");
            
            // Play death sound at position (independent of GameObject)
            if (deathSFX != null)
            {
                AudioSource.PlayClipAtPoint(deathSFX, transform.position, deathVolume);
            }
            
            Destroy(gameObject);
        }
    }

    // Getter for attackPower
    public int AttackPower => attackPower;
}
