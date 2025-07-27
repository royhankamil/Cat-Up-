using UnityEngine;
using UnityEngine.InputSystem;

public class RotatingObject : MonoBehaviour
{
    public InputActionReference mousePositionAction;
    public InputActionReference mouseClickAction;
    public InputActionReference rotateLeftAction;
    public InputActionReference rotateRightAction;

    private bool isDragging = false;
    private Vector3 offset;

    private void OnEnable()
    {
        mousePositionAction.action.Enable();
        mouseClickAction.action.Enable();
        rotateLeftAction.action.Enable();
        rotateRightAction.action.Enable();
        Debug.Log("Input actions enabled!");
    }

    private void OnDisable()
    {
        mousePositionAction.action.Disable();
        mouseClickAction.action.Disable();
        rotateLeftAction.action.Disable();
        rotateRightAction.action.Disable();
    }

    void Update()
    {
        Vector3 mouseScreenPos = mousePositionAction.action.ReadValue<Vector2>();
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, 0f));
        Vector2 mouseWorldPos2D = new Vector2(mouseWorldPos.x, mouseWorldPos.y);

        // Mouse down: check if mouse is over this object
        if (mouseClickAction.action.WasPressedThisFrame())
        {
            Debug.Log("Mouse click action detected!");
            Collider2D hit = Physics2D.OverlapPoint(mouseWorldPos2D);
            if (hit != null)
                Debug.Log("Collider hit: " + hit.gameObject.name);
            if (hit != null && hit.gameObject == gameObject)
            {
                Debug.Log("object clicked");
                isDragging = true;
                offset = transform.position - (Vector3)mouseWorldPos2D;
            }
        }

        // Mouse up: stop dragging
        if (mouseClickAction.action.WasReleasedThisFrame())
        {
            isDragging = false;
        }

        // Dragging: move object
        if (isDragging)
        {
            transform.position = mouseWorldPos2D + (Vector2)offset;

            // Rotation
            if (rotateRightAction.action.IsPressed())
            {
                transform.Rotate(Vector3.forward, -100f * Time.deltaTime); // 2D rotation
            }
            if (rotateLeftAction.action.IsPressed())
            {
                transform.Rotate(Vector3.forward, 100f * Time.deltaTime); // 2D rotation
            }
        }
        Debug.DrawLine(mouseWorldPos2D, mouseWorldPos2D + Vector2.up * 0.5f, Color.red, 0.1f);
    }
}
