using UnityEngine;
using UnityEngine.InputSystem;

public class ItemSpawner : MonoBehaviour
{
    public GameObject itemPrefab;
    public InputActionReference mousePositionAction;
    public InputActionReference mouseClickAction;
    public RectTransform imageRectTransform;
    
    private bool isDragging = false;
    private bool hasSpawned = false;
    private Vector3 lastMousePosition;
    private GameObject currentItem;

    private void OnEnable()
    {
        mousePositionAction.action.Enable();
        mouseClickAction.action.Enable();
    }

    private void OnDisable()
    {
        mousePositionAction.action.Disable();
        mouseClickAction.action.Disable();
    }

    void Update()
    {
        if (GameManager.IsPlay)
            return;

        Vector3 mouseScreenPos = mousePositionAction.action.ReadValue<Vector2>();
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, 0f));
        
        // Start dragging
        if (mouseClickAction.action.WasPressedThisFrame())
        {
            // Check if mouse is over this specific image using UI rect
            bool isMouseOverImage = RectTransformUtility.RectangleContainsScreenPoint(
                imageRectTransform,
                mouseScreenPos,
                Camera.main
            );
            if (isMouseOverImage)
            {
                isDragging = true;
                hasSpawned = false;
                lastMousePosition = mouseScreenPos;
            }
        }
        
        // Stop dragging
        if (mouseClickAction.action.WasReleasedThisFrame())
        {
            isDragging = false;
            currentItem = null; // Release the item
        }
        
        // Check for drag leaving the image during dragging
        if (isDragging && !hasSpawned)
        {
            bool isMouseOverImage = RectTransformUtility.RectangleContainsScreenPoint(
                imageRectTransform,
                mouseScreenPos,
                Camera.main
            );
            Debug.Log($"isDragging: {isDragging}, hasSpawned: {hasSpawned}, isMouseOverImage: {isMouseOverImage}, mouseScreenPos: {mouseScreenPos}");
            if (!isMouseOverImage)
            {
                SpawnItem(mouseWorldPos);
                hasSpawned = true;
            }
        }
        
        // Make the spawned item follow the mouse
        if (isDragging && currentItem != null)
        {
            currentItem.transform.position = new Vector3(mouseWorldPos.x, mouseWorldPos.y, 0f);
        }
    }
    
    void SpawnItem(Vector3 position)
    {
        if (itemPrefab != null)
        {
            currentItem = Instantiate(itemPrefab, position, Quaternion.identity);
            currentItem.GetComponent<RotatingObject>().EnableDragging();
            currentItem.name = "item";
            Debug.Log("Item spawned at: " + position);
        }
        else
        {
            Debug.LogWarning("Item prefab not assigned!");
        }
    }
} 