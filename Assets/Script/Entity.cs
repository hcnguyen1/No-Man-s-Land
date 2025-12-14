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
    
    public SpriteRenderer spriteRenderer;
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
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected virtual void Initialize()
    {
        health = maxHealth;
    }

    // Ensures Die() is executed only once
    protected bool hasDied = false;

    public virtual void TakeDamage(int damage)
    {
        health -= damage;
        PlayGetHitSFX();
        
        if (health <= 0)
        {
            OnZeroHealth();
        }

        StartCoroutine(TakeDamageFlash());
    }

    protected virtual void PlayGetHitSFX()
    {
        if (audioSource != null && getHitSFX != null)
        {
            audioSource.PlayOneShot(getHitSFX, getHitVolume);
        }
    }

    // Hook for subclasses to handle death flow (animations, drops)
    protected virtual void OnZeroHealth()
    {
        // Default behavior: immediately die
        Die();
    }

    protected virtual void Die()
    {
        // Debug log of that entity's name has died
        if (health <= 0 && !hasDied)
        {
            hasDied = true;
            // Play death sound at position (independent of GameObject)
            if (deathSFX != null)
            {
                AudioSource.PlayClipAtPoint(deathSFX, transform.position, deathVolume);
            }
            
            Destroy(gameObject);
        }
    }

    // Animation Event entry point to finish death
    public void OnDeathAnimationComplete()
    {
        Die();
    }

    // Getter for attackPower
    public int AttackPower => attackPower;

    // Method to modify attack power (for equipment bonuses)
    public void ModifyAttackPower(int amount)
    {
        attackPower += amount;
    }

    // Flashes red when taking damage (In case there is no take damage animation)
    private IEnumerator TakeDamageFlash()
    {
        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = originalColor;
    }
}
