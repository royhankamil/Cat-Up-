using UnityEngine;
using UnityEngine.InputSystem;

public class ItemSpawner : MonoBehaviour
{
    public GameObject itemPrefab;
    public InputActionReference mousePositionAction;
    public InputActionReference mouseClickAction;
    public RectTransform imageRectTransform;
    
    // Public property for RotatingObject to access
    public RectTransform ImageRectTransform => imageRectTransform;
    
    private bool isDragging = false;
    private bool hasSpawned = false;
    private Vector3 lastMousePosition;
    private GameObject currentItem;
    private bool imageHidden = false;

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
            hasSpawned = false; // Reset spawn state
            currentItem = null; // Release the item
        }
        
        // Check for drag leaving the image during dragging (only once)
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
                HideImage();
            }
        }
        
        // Check if spawned item is dragged back to UI area (only when not dragging from ItemSpawner)
        if (hasSpawned && currentItem != null && !isDragging)
        {
            bool isMouseOverImage = RectTransformUtility.RectangleContainsScreenPoint(
                imageRectTransform,
                mouseScreenPos,
                Camera.main
            );
            if (isMouseOverImage)
            {
                ShowImage();
                Destroy(currentItem);
                currentItem = null;
                hasSpawned = false;
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
            RotatingObject rotatingScript = currentItem.GetComponent<RotatingObject>();
            if (rotatingScript != null)
            {
                rotatingScript.EnableDragging();
                // Set reference to this ItemSpawner so RotatingObject can communicate back
                rotatingScript.SetItemSpawner(this);
            }
            currentItem.name = "item";
            Debug.Log("Item spawned at: " + position);
        }
        else
        {
            Debug.LogWarning("Item prefab not assigned!");
        }
    }
    
    void HideImage()
    {
        if (!imageHidden)
        {
            // Hide the image by setting alpha to 0
            CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
            canvasGroup.alpha = 0f;
            imageHidden = true;
            Debug.Log("Image hidden");
        }
    }
    
    void ShowImage()
    {
        if (imageHidden)
        {
            // Show the image by setting alpha back to 1
            CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
            }
            imageHidden = false;
            Debug.Log("Image shown");
        }
    }
    
    // Called by RotatingObject when item is dragged back to UI area
    public void OnItemReturnedToUI()
    {
        if (hasSpawned && currentItem != null)
        {
            ShowImage();
            Destroy(currentItem);
            currentItem = null;
            hasSpawned = false;
            Debug.Log("Item returned to UI and destroyed");
        }
    }
} 