using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class Player : Entity
// Assign your health potion ItemSO in the Inspector
{
    public GameObject hitbox;
    [SerializeField] private float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator animator;

    [SerializeField] private AudioClip attackSFX;
    [SerializeField] [Range(0f, 1f)] private float attackVolume = 0.5f;


    [Header("Hunger and Thirst Stats")]
    public float maxHunger;
    public float hunger;
    public float maxThirst;
    public float thirst;

    // Hunger and Thirst Decay Rates
    public float hungerDecayRate;
    public float thirstDecayRate;

    private bool isTakingNoHungerDamage = false;
    [SerializeField] int noHungerDamage = 1;
    [SerializeField] int noHungerDamageInterval = 1; // Seconds

    private bool isTakingNoThirstPenalty = false;
    [SerializeField] float noThirstStatReduction = 0.5f;



    private float baseMoveSpeed;
    private int baseAttackPower;

    // Currency System
    [SerializeField]
    private int _currency = 0;
    public int currency
    {
        get => _currency;
        set => _currency = value;
    }

    // Rolling Mechanic
    public bool canRoll = true;
    public bool isRolling = false;
    public bool isInvincible = false;
    private float rollCooldownTime = 2f;
    private float rollCooldownTimer = 0f;
    private float rollDuration = 0.733f; // Match your animation length
    private float rollTimer = 0f;

    public bool canOpenCraftingMenu = false;

    private PlayerInput playerInput;
    private Vector2 lastMoveDir = Vector2.down; // Default facing down

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        health = maxHealth;
        hunger = maxHunger;
        thirst = maxThirst;

        baseMoveSpeed = moveSpeed;
        baseAttackPower = attackPower;
    }
    void Update()
    {
        // Decay if in level scene
        if (SceneManager.GetActiveScene().name.Contains("Level"))
        {
            decayHungerAndThirst();
        }

        // Handle roll cooldown
        if (!canRoll)
        {
            rollCooldownTimer -= Time.deltaTime;
            if (rollCooldownTimer <= 0f)
            {
                canRoll = true;
            }
        }

        if (isRolling)
        {
            // Keep moving during roll
            rb.velocity = lastMoveDir * moveSpeed * 1.5f; // 1.5x speed during roll
            
            // Timer-based roll end (more reliable than animation events)
            rollTimer -= Time.deltaTime;
            if (rollTimer <= 0f)
            {
                OnRollEnd();
            }
            return; // ignore all other input
        }

        rb.velocity = moveInput * moveSpeed;

        CheckAndResetAttackState();

        // Update lastMoveDir based on movement input
        Vector2 move = playerInput.actions["Move"].ReadValue<Vector2>();
        if (move != Vector2.zero)
            lastMoveDir = move.normalized;

        // Only allow attack if not clicking on UI
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            // Only attack if not already attacking
            if (!animator.GetBool("isAttacking"))
            {
                animator.SetBool("isAttacking", true);
                animator.SetFloat("AttackX", lastMoveDir.x); // Use your last input direction
                animator.SetFloat("AttackY", lastMoveDir.y);
                PlayAttackSFX();
            }
        }

    }

    private void OnRoll(InputAction.CallbackContext context)
    {
        if (!isRolling && canRoll)
        {
            animator.SetFloat("RollX", lastMoveDir.x);
            animator.SetFloat("RollY", lastMoveDir.y);
            animator.SetBool("isRolling", true);

            isRolling = true;
            isInvincible = true;
            canRoll = false;
            rollCooldownTimer = rollCooldownTime; // Start 2-second cooldown
            rollTimer = rollDuration; // Start roll animation timer
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        animator.SetBool("isWalking", true); // animator sets walking to true at first when we move, then will check boolean value below.

        if (context.canceled) // when we stop walking we tell the animator to switch back to idle.
        {
            animator.SetBool("isWalking", false);
            animator.SetFloat("LastInputX", moveInput.x);
            animator.SetFloat("LastInputY", moveInput.y);
        }

        moveInput = context.ReadValue<Vector2>();

        animator.SetFloat("InputX", moveInput.x); // the x input is whatever moveinput we use for x
        animator.SetFloat("InputY", moveInput.y); // the y input also follows the same rule
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("CraftingBench"))
        {
            canOpenCraftingMenu = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("CraftingBench"))
        {
            canOpenCraftingMenu = false;
        }
    }

    private void decayHungerAndThirst()
    {
        hunger -= hungerDecayRate * Time.deltaTime;
        thirst -= thirstDecayRate * Time.deltaTime;

        hunger = Mathf.Clamp(hunger, 0, maxHunger);
        thirst = Mathf.Clamp(thirst, 0, maxThirst);

        if(hunger <= 0 && !isTakingNoHungerDamage)
        {
            StartCoroutine(NoHungerDamageOverTime());
        }
        NoThirst();
    }


    public void EnableHitbox()
    {
        hitbox.SetActive(true);
    }
    public void DisableHitbox()
    {
        hitbox.SetActive(false);
    }

    public override void TakeDamage(int damage)
    {
        // invincibility during rolls or some other function that can call it. 
        if (isInvincible)
        {
            return;
        }
        base.TakeDamage(damage);
    }

    private void OnFire(InputAction.CallbackContext context)
    {
        animator.SetBool("isAttacking", true);
        animator.SetFloat("AttackX", lastMoveDir.x);
        animator.SetFloat("AttackY", lastMoveDir.y);
        PlayAttackSFX();
    }

    // Call this from an Animation Event at the end of your attack animation
    public void EndAttack()
    {
        animator.SetBool("isAttacking", false);
    }

    public void OnRollEnd()
    {
        if (!isRolling) return; // Already ended, ignore duplicate calls
        animator.SetBool("isRolling", false);
        isRolling = false;
        isInvincible = false;
    }

    // The same player will exist across every level
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        playerInput.actions["Roll"].performed += OnRoll;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        playerInput.actions["Roll"].performed -= OnRoll;
    }

    // Hard-coded player position for Level1 entry
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        int locationX = -30;
        int locationY = -3;
        if (scene.name == "Level1")
        {
            transform.position = new Vector3(locationX, locationY, 0);
        }
    }

    // checks to see if your character is stuck in isAttacking Lock 
    private void CheckAndResetAttackState()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Attack") && animator.GetBool("isAttacking") == true && stateInfo.normalizedTime > 1f)
        {
            animator.SetBool("isAttacking", false);
        }
    }

    private void PlayAttackSFX()
    {
        if (audioSource != null && attackSFX != null)
        {
            audioSource.PlayOneShot(attackSFX, attackVolume);
        }
    }

    private IEnumerator NoHungerDamageOverTime()
    {
        isTakingNoHungerDamage = true;
        while (hunger <= 0)
        {
            TakeDamage(noHungerDamage);
            yield return new WaitForSeconds(noHungerDamageInterval);
        }
        isTakingNoHungerDamage = false;
    }

    private void NoThirst()
    {
        if (thirst <= 0 && !isTakingNoThirstPenalty)
        {
            isTakingNoThirstPenalty = true;
            moveSpeed = baseMoveSpeed - (baseMoveSpeed * noThirstStatReduction); // 5 reduced by (5 * 0.5) = 2.5
            attackPower = baseAttackPower - (int)(baseAttackPower * noThirstStatReduction);
        }
        else if (thirst > 0 && isTakingNoThirstPenalty)
        {
            isTakingNoThirstPenalty = false;
            moveSpeed = baseMoveSpeed;
            attackPower = baseAttackPower;
        }
    }


}


