using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Entity
{
    [SerializeField] private float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator animator;

    public bool canOpenCraftingMenu;
    [SerializeField] private GameObject craftingMenu;
    [SerializeField] private GameObject craftButton;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        rb.velocity = moveInput * moveSpeed;


        // this lets us open the crafting menu by letting us press the craft button in our inventories when next to craft bench. 
        if (canOpenCraftingMenu)
        {
            craftButton.SetActive(true);
        }
        else
        {
            craftingMenu.SetActive(false);
            craftButton.SetActive(false);
        }
    }

    public void OpenCraftingMenu()
    {
        craftingMenu.SetActive(true);
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
}

