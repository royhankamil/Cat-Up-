using UnityEngine;

public class RotatingObject : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 offset;
    
    void Update()
    {
        // Mouse down: check if mouse is over this object
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mouseWorldPos2D = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D hit = Physics2D.OverlapPoint(mouseWorldPos2D);
            if (hit != null && hit.gameObject == gameObject)
            {
                isDragging = true;
                offset = transform.position - (Vector3)mouseWorldPos2D;
            }
        }

        // Mouse up: stop dragging
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        // Dragging: move object
        if (isDragging)
        {
            Vector2 mouseWorldPos2D = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = mouseWorldPos2D + (Vector2)offset;

            // Rotation
            if (Input.GetKey(KeyCode.RightArrow))
            {
                transform.Rotate(Vector3.forward, -100f * Time.deltaTime); // 2D rotation
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                transform.Rotate(Vector3.forward, 100f * Time.deltaTime); // 2D rotation
            }
        }
    }
}
