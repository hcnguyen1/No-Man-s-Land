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

    public float maxHunger;
    public float hunger;
    public float maxThirst;
    public float thirst;

    // Hunger and Thirst Decay Rates
    public float hungerDecayRate;
    public float thirstDecayRate;

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



    // HEALTH ITEMS 
    public ItemSO healthPotion;
    public ItemSO healthSyringe;
    private InventoryManager inventoryManager;
    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        // inventoryManager should be assigned in the Inspector for reliability
    }

    private void OnUseHealthPotion(InputAction.CallbackContext context)
    {
        if (healthPotion != null && healthPotion.UseItem())
            return;
        if (healthSyringe != null)
            healthSyringe.UseItem();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        health = maxHealth;
        hunger = maxHunger;
        thirst = maxThirst;
    }
    void Update()
    {
        decayHungerAndThirst(); // Always decay, even when rolling

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
            animator.SetBool("isAttacking", true);
            animator.SetFloat("AttackX", lastMoveDir.x); // Use your last input direction
            animator.SetFloat("AttackY", lastMoveDir.y);
        }

    }

    private void OnRoll(InputAction.CallbackContext context)
    {
        Debug.Log($"OnRoll called! isRolling={isRolling}, canRoll={canRoll}");
        
        if (!isRolling && canRoll)
        {
            Debug.Log($"Rolling! Direction: ({lastMoveDir.x}, {lastMoveDir.y})");
            animator.SetFloat("RollX", lastMoveDir.x);
            animator.SetFloat("RollY", lastMoveDir.y);
            animator.SetBool("isRolling", true);

            isRolling = true;
            isInvincible = true;
            canRoll = false;
            rollCooldownTimer = rollCooldownTime; // Start 2-second cooldown
            rollTimer = rollDuration; // Start roll animation timer
        }
        else
        {
            Debug.Log("Roll blocked - already rolling or on cooldown");
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
            Debug.Log("Player is invincible! Damage blocked.");
            return;
        }
        base.TakeDamage(damage);
    }

    private void OnFire(InputAction.CallbackContext context)
    {
        Debug.Log("Fire action performed!");
        animator.SetBool("isAttacking", true);
        animator.SetFloat("AttackX", lastMoveDir.x);
        animator.SetFloat("AttackY", lastMoveDir.y);
    }

    // Call this from an Animation Event at the end of your attack animation
    public void EndAttack()
    {
        animator.SetBool("isAttacking", false);
    }

    public void OnRollEnd()
    {
        if (!isRolling) return; // Already ended, ignore duplicate calls
        
        Debug.Log("OnRollEnd called - resetting roll state");
        animator.SetBool("isRolling", false);
        isRolling = false;
        isInvincible = false;
    }

    // The same player will exist across every level
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        playerInput.actions["UseHealthPotion"].performed += OnUseHealthPotion;
        playerInput.actions["Roll"].performed += OnRoll;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        playerInput.actions["UseHealthPotion"].performed -= OnUseHealthPotion;
        playerInput.actions["Roll"].performed -= OnRoll;
    }

    // Hard-coded player position for Level1 entry
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        int locationX = 2;
        int locationY = 2;
        if (scene.name == "Level1")
        {
            transform.position = new Vector3(locationX, locationY, 0);
            Debug.Log("Player spawned at Level1 at (" + locationX + ", " + locationY + ")");
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


}


