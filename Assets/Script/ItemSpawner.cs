using UnityEngine;
using UnityEngine.InputSystem;

public class ItemSpawner : MonoBehaviour
{
    public GameObject itemPrefab;
    public InputActionReference mousePositionAction;
    public InputActionReference mouseClickAction;
    public RectTransform imageRectTransform;
    public ObjectType objectType;
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
        float spawnZ = Mathf.Abs(Camera.main.transform.position.z);
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, spawnZ));

        // --- Start Dragging ---
        if (mouseClickAction.action.WasPressedThisFrame())
        {
            // Check if mouse is over this specific image's RectTransform
            bool isMouseOverImage = RectTransformUtility.RectangleContainsScreenPoint(
                transform as RectTransform,
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

        // --- Stop Dragging ---
        if (mouseClickAction.action.WasReleasedThisFrame())
        {
            isDragging = false;
            hasSpawned = false; // Reset spawn state
            currentItem = null; // Release the item reference
        }

        // --- Handle All Drag Logic (Spawning and Moving) ---
        if (isDragging)
        {
            // 1. Check if we need to SPAWN the item.
            // This happens only once when the cursor leaves the UI button while dragging.
            if (!hasSpawned)
            {
                bool isMouseOverImage = RectTransformUtility.RectangleContainsScreenPoint(
                    imageRectTransform,
                    mouseScreenPos,
                    Camera.main
                );
                if (!isMouseOverImage)
                {
                    SpawnItem(mouseWorldPos);
                    hasSpawned = true; // Mark as spawned so we don't do this again
                    HideImage();
                }
            }

            // 2. If the item is spawned, make it MOVE with the mouse.
            // This is the key change for smooth dragging.
            if (currentItem != null)
            {
                // Set the z-coordinate to 0 to ensure it's on the main plane
                currentItem.transform.position = new Vector3(mouseWorldPos.x, mouseWorldPos.y, 0f);
            }
        }

        // Check if spawned item is dragged back to UI area (functionality preserved as is)
        if (hasSpawned && currentItem != null && !isDragging)
        {
            bool isMouseOverImage = RectTransformUtility.RectangleContainsScreenPoint(
                imageRectTransform,
                mouseScreenPos,
                Camera.main
            );
            // if (isMouseOverImage)
            // {
            //     ShowImage();
            //     Destroy(currentItem);
            //     currentItem = null;
            //     hasSpawned = false;
            // }
        }
    }

    void SpawnItem(Vector3 position)
    {
        if (itemPrefab != null)
        {
            currentItem = Instantiate(itemPrefab, position, Quaternion.identity);
            RotatingObject rotatingScript = currentItem.GetComponent<RotatingObject>();
            // rotatingScript.ImageReference = this.gameObject;
            if (rotatingScript != null)
            {
                rotatingScript.EnableDragging();
                // Set reference to this ItemSpawner so RotatingObject can communicate back
                rotatingScript.SetItemSpawner(this);
            }
            // currentItem.name = "item";
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
            // // Hide the image by setting alpha to 0
            CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }

            transform.SetSiblingIndex(transform.parent.childCount - 1);

            canvasGroup.alpha = 0f;
            imageHidden = true;
            // Debug.Log("Image hidden");
            // gameObject.SetActive(false); // Hide the entire GameObject
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
            transform.SetSiblingIndex(0);
            // Debug.Log("Image shown");
        }
    }

    // Called by RotatingObject when item is dragged back to UI area
    public void OnItemReturnedToUI(GameObject obj)
    {
        // if (hasSpawned && currentItem != null)
        // {
        ShowImage();
        Destroy(obj);
        currentItem = null;
        isDragging = false;
        hasSpawned = false;
        Debug.Log("Item returned to UI and destroyed");
        // }
    }
}