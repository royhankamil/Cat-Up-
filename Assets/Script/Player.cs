using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 7f;

    [Header("Physics & Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    [Header("State Timers")]
    public float timeToSleep = 2.5f;

    // Private component references
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;

    // Input state variables
    private Vector2 moveInput;
    private bool isJumpHeld = false;
    private bool jumpTriggered = false; 
    private bool sitTriggered = false;
    
    // State tracking variables
    private bool isGrounded;
    private float sitTimer = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // --- Input System Events ---
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            jumpTriggered = true;
            isJumpHeld = true;
            AudioManager.Instance.PlaySfx("Jump");
        }
        else if (context.canceled)
        {
            isJumpHeld = false;
        }
    }

    public void OnSit(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            sitTriggered = true;
        }
    }

    private void Update()
    {
        HandleSpriteFlip();
        HandleAnimationsAndStates();
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // If sitting or sleeping, stop horizontal movement.
        if (anim.GetBool("isSit") || anim.GetBool("isSleep"))
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }
        
        HandleMovement();
        HandleJump();
        HandleBetterJump();
    }

    // --- Logic Handling Methods ---

    private void HandleMovement()
    {
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
    }
    
    private void HandleSpriteFlip()
    {
        if (moveInput.x > 0.1f)
        {
            spriteRenderer.flipX = false;
        }
        else if (moveInput.x < -0.1f)
        {
            spriteRenderer.flipX = true;
        }
    }

    private void HandleJump()
    {
        if (jumpTriggered && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
        // Consume the trigger so it only happens once per press
        jumpTriggered = false; 
    }

    private void HandleBetterJump()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.gravityScale = fallMultiplier;
        }
        else if (rb.linearVelocity.y > 0 && !isJumpHeld)
        {
            rb.gravityScale = lowJumpMultiplier;
        }
        else
        {
            rb.gravityScale = 1f;
        }
    }

    // CHANGED: Removed the wake-up delay for immediate response
    private void HandleAnimationsAndStates()
    {
        // --- Sit & Sleep Logic ---
        // If 'S' was just pressed, we toggle the sit state.
        if (sitTriggered)
        {
            // If we are already sitting or sleeping, stand up.
            if (anim.GetBool("isSit") || anim.GetBool("isSleep"))
            {
                anim.SetBool("isSit", false);
                anim.SetBool("isSleep", false);
                sitTimer = 0f;
            }
            // Otherwise, if we are on the ground and not moving, sit down.
            else if (isGrounded && Mathf.Abs(moveInput.x) < 0.1f)
            {
                anim.SetBool("isSit", true);
                AudioManager.Instance.PlaySfx("SitDown");
            }
        }
        sitTriggered = false;

        if (anim.GetBool("isSit"))
        {
            sitTimer += Time.deltaTime;
            if (sitTimer > timeToSleep)
            {
                anim.SetBool("isSleep", true);
            }
        }
        
        // --- Breaking Out of Sit/Sleep with Movement ---
        // If we move or jump, we should stand up.
        bool shouldBreakSit = (Mathf.Abs(moveInput.x) > 0.1f) || (jumpTriggered && isGrounded);
        if (shouldBreakSit)
        {
            if (anim.GetBool("isSit") || anim.GetBool("isSleep"))
            {
                anim.SetBool("isSit", false);
                anim.SetBool("isSleep", false);
                sitTimer = 0f;
                AudioManager.Instance.PlaySfx("StandUp");
            }
        }

        // --- Walk, Jump, and Fall Animations ---
        anim.SetBool("isWalk", Mathf.Abs(moveInput.x) > 0.1f && isGrounded);

        if (!isGrounded)
        {
            if (rb.linearVelocity.y > 0.1f)
            {
                anim.SetBool("isJump", true);
                anim.SetBool("isFall", false);
            }
            else if (rb.linearVelocity.y < -0.1f)
            {
                anim.SetBool("isJump", false);
                anim.SetBool("isFall", true);
            }
        }
        else
        {
            anim.SetBool("isJump", false);
            anim.SetBool("isFall", false);
        }
    }
    
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
