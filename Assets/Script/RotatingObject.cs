using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;

public class RotatingObject : MonoBehaviour
{
    // Input Actions
    public InputActionReference mousePositionAction;
    public InputActionReference mouseClickAction;
    public InputActionReference rotateLeftAction;
    public InputActionReference rotateRightAction;
    public InputActionReference scrollAction;
    public float mass = 1f;

    // --- SMOOTHNESS: Configurable speeds for smoothing ---
    [Header("Smoothing Settings")]
    [Tooltip("How quickly the object follows the mouse. Higher values are faster.")]
    [SerializeField] private float dragSmoothSpeed = 15f;
    [Tooltip("How fast the object rotates with keys.")]
    [SerializeField] private float keyRotationSpeed = 400f;
    [Tooltip("How fast the object rotates with the scroll wheel.")]
    public float scrollRotationSpeed = 600f;

    [SerializeField] private ObjectType objectType;
    private bool isDragging = false;
    private Vector3 offset;
    private ItemSpawner itemSpawner; // Reference to ItemSpawner

    // --- SMOOTHNESS: Target position for smooth dragging ---
    private Vector3 targetPosition;

    private void OnEnable()
    {
        mousePositionAction.action.Enable();
        mouseClickAction.action.Enable();
        rotateLeftAction.action.Enable();
        rotateRightAction.action.Enable();
        if (scrollAction != null) scrollAction.action.Enable();
        Debug.Log("Input actions enabled!");
    }

    public void EnableDragging()
    {
        isDragging = true;
        // When dragging is enabled programmatically, we need to calculate the initial offset
        // and set the initial target position to the current position to avoid a jump.
        Vector3 mouseScreenPos = mousePositionAction.action.ReadValue<Vector2>();
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, Mathf.Abs(Camera.main.transform.position.z)));
        offset = transform.position - mouseWorldPos;
        targetPosition = transform.position; // Start at the current position
    }

    public void SetItemSpawner(ItemSpawner spawner)
    {
        itemSpawner = spawner;
    }

    private void OnDisable()
    {
        // It's good practice to disable actions to prevent potential memory leaks.
        // You can uncomment these if you are sure they won't be needed elsewhere.
        // mousePositionAction.action.Disable();
        // mouseClickAction.action.Disable();
        // rotateLeftAction.action.Disable();
        // rotateRightAction.action.Disable();
        // if (scrollAction != null) scrollAction.action.Disable();
    }

    void Update()
    {
        if (GameManager.IsPlay)
        {
            GetComponent<Rigidbody2D>().mass = mass;
            if (gameObject.layer == LayerMask.NameToLayer("DraggingItem"))
            {
                gameObject.layer = LayerMask.NameToLayer("Item");
            }
            return;
        }

        // Calculate mouse position in world space
        Vector3 mouseScreenPos = mousePositionAction.action.ReadValue<Vector2>();
        float z = Mathf.Abs(Camera.main.transform.position.z);
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, z));

        // Mouse down: check if mouse is over this object
        if (mouseClickAction.action.WasPressedThisFrame())
        {
            Collider2D hit = Physics2D.OverlapPoint(mouseWorldPos);
            if (hit != null && hit.gameObject == gameObject)
            {
                isDragging = true;
                // Calculate the offset from the object's center to the mouse click position
                offset = transform.position - mouseWorldPos;
                // Set the initial target position to prevent snapping
                targetPosition = transform.position;
                gameObject.layer = LayerMask.NameToLayer("DraggingItem");
            }
        }

        // Mouse up: stop dragging
        if (mouseClickAction.action.WasReleasedThisFrame())
        {
            if (isDragging) // Only change layer if it was being dragged
            {
                isDragging = false;
                gameObject.layer = LayerMask.NameToLayer("Item");
            }
        }

        // Dragging logic
        if (isDragging)
        {
            // --- SMOOTHNESS: Update the target position to follow the mouse ---
            targetPosition = mouseWorldPos + offset;

            // --- SMOOTHNESS: Smoothly move the object towards the target position ---
            // Vector3.Lerp interpolates between the current position and the target position.
            transform.position = Vector3.Lerp(transform.position, targetPosition, dragSmoothSpeed * Time.deltaTime);


            // --- ROTATION FIX: Calculate total rotation input for this frame ---
            float rotationInput = 0f;
            if (rotateRightAction.action.IsPressed())
            {
                rotationInput -= keyRotationSpeed;
            }
            if (rotateLeftAction.action.IsPressed())
            {
                rotationInput += keyRotationSpeed;
            }
            if (scrollAction != null)
            {
                float scrollValue = scrollAction.action.ReadValue<Vector2>().y;
                if (Mathf.Abs(scrollValue) > 0.01f)
                {
                    // Apply scroll rotation directly
                    rotationInput -= scrollValue * scrollRotationSpeed;
                }
            }

            // --- ROTATION FIX: Apply rotation directly. This is frame-rate independent and responsive. ---
            if (Mathf.Abs(rotationInput) > 0.01f)
            {
                // Use transform.Rotate for continuous rotation.
                // We multiply by Time.deltaTime here once to make it a smooth, frame-rate independent speed.
                transform.Rotate(Vector3.forward, rotationInput * Time.deltaTime);
            }

            // Check if item is dragged back to UI area
            HandleReturnToUI(mouseScreenPos);
        }

        Debug.DrawLine(mouseWorldPos, mouseWorldPos + Vector3.up * 0.5f, Color.red, 0.1f);
    }

    private void HandleReturnToUI(Vector3 currentMouseScreenPos)
    {
        if (itemSpawner != null && itemSpawner.ImageRectTransform != null)
        {
            bool isOverUI = RectTransformUtility.RectangleContainsScreenPoint(
                itemSpawner.ImageRectTransform,
                currentMouseScreenPos,
                null // Use null for Screen Space - Overlay canvas
            );

            if (isOverUI)
            {
                itemSpawner.OnItemReturnedToUI(this.gameObject);
                // The spawner will handle destroying this object, so no need to do anything else here.
            }
        }
    }
}
