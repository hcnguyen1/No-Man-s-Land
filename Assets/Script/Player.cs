using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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

    public bool canOpenCraftingMenu = false;

    private PlayerInput playerInput;
    private Vector2 lastMoveDir = Vector2.down; // Default facing down



    // HEALTH ITEMS 
    public ItemSO healthPotion;

    public ItemSO healthSyringe;
    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
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
        rb.velocity = moveInput * moveSpeed;

        decayHungerAndThirst();

        // Update lastMoveDir based on movement input
        Vector2 move = playerInput.actions["Move"].ReadValue<Vector2>();
        if (move != Vector2.zero)
            lastMoveDir = move.normalized;

        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            animator.SetBool("isAttacking", true);
            animator.SetFloat("AttackX", lastMoveDir.x); // Use your last input direction
            animator.SetFloat("AttackY", lastMoveDir.y);
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

    // The same player will exist across every level
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        playerInput.actions["UseHealthPotion"].performed += OnUseHealthPotion;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        playerInput.actions["UseHealthPotion"].performed -= OnUseHealthPotion;
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
}


