using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Wall Check")]
    public float wallCheckDistance = 0.1f;
    public LayerMask wallLayer;

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
    // MODIFIED: Added a public string to display the formatted countdown.
    // This value is read-only and is updated by the script.
    [Tooltip("Time remaining until the player falls asleep.")]
    public string sleepCountdownDisplay;

    // Private component references
    private Rigidbody2D rb;
    public Animator anim;
    private SpriteRenderer spriteRenderer;

    // Input state variables
    private Vector2 moveInput;
    private bool isJumpHeld = false;
    private bool jumpTriggered = false;
    public bool sitTriggered { get; private set; } = false;

    // State tracking variables
    private bool isGrounded;
    // MODIFIED: The sitTimer is now private, as its only purpose is for internal logic.
    private float sitTimer = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        // ADDED: Initialize the display timer on start.
        sleepCountdownDisplay = timeToSleep.ToString("F1") + "s";
    }

    // --- Input System Events ---
    public void OnMove(InputAction.CallbackContext context)
    {
        if (!GameManager.IsPlay)
        {
            if (!ItemSpawner.IsDragging)
            {
                NotifyManager.Instance.TriggerNotify("Invalid Movement");
            }
            moveInput = Vector2.zero;
            return;
        }
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!GameManager.IsPlay) return;

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
        if (!GameManager.IsPlay) return;

        if (context.started)
        {
            sitTriggered = true;
        }
    }

    private void Update()
    {
        if (!GameManager.IsPlay) return;

        HandleSpriteFlip();
        HandleAnimationsAndStates();
    }

    private void FixedUpdate()
    {
        if (!GameManager.IsPlay)
        {
            rb.linearVelocity = Vector2.zero;
            anim.SetBool("isWalk", false);
            return;
        }

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (anim.GetBool("isSit") || anim.GetBool("isSleep"))
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        HandleMovement();
        HandleJump();
        HandleBetterJump();
        HandleRotation();
    }

    // --- Logic Handling Methods ---

    private void HandleMovement()
    {
        if (!isGrounded && IsTouchingWall())
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
        }
    }

    private void HandleRotation()
    {
        float z = transform.rotation.eulerAngles.z;
        if (z > 180f) z -= 360f;
        z = Mathf.Clamp(z, -45f, 45f);
        transform.rotation = Quaternion.Euler(0, 0, z);
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
    private bool IsTouchingWall()
    {
        Vector2 direction = moveInput.x > 0 ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, wallCheckDistance, wallLayer);
        return hit.collider != null;
    }

    private void HandleJump()
    {
        if (jumpTriggered && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
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

    private void HandleAnimationsAndStates()
    {
        // --- Sit & Sleep Logic ---
        if (sitTriggered && isGrounded && Mathf.Abs(moveInput.x) < 0.1f && !anim.GetBool("isSit"))
        {
            anim.SetBool("isSit", true);
            sitTimer = 0f;
            AudioManager.Instance.PlaySfx("SitDown");
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

        // --- MODIFIED: Update the countdown display string ---
        if (anim.GetBool("isSit") && !anim.GetBool("isSleep"))
        {
            // Calculate remaining time
            float remainingTime = timeToSleep - sitTimer;
            // Ensure the display doesn't show a negative number
            remainingTime = Mathf.Max(0f, remainingTime);
            // Format the string with one decimal place and "s"
            sleepCountdownDisplay = remainingTime.ToString("F1") + "s";
        }
        else
        {
            // When not in the "sitting" countdown phase, show the full time.
            sleepCountdownDisplay = timeToSleep.ToString("F1") + "s";
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