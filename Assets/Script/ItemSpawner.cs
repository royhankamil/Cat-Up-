using UnityEngine;
using UnityEngine.InputSystem;

public class ItemSpawner : MonoBehaviour
{
    public GameObject itemPrefab;
    public InputActionReference mousePositionAction;
    public InputActionReference mouseClickAction;
    public RectTransform imageRectTransform;
    public ObjectType objectType;
    public RectTransform ImageRectTransform => imageRectTransform;

    // --- STATIC DRAG MANAGEMENT ---
    // This static reference holds the SPECIFIC spawner instance that is currently dragging.
    private static ItemSpawner currentlyDraggingSpawner;

    // This public static property fulfills the request. It's true if ANY spawner is dragging.
    // It's a read-only property (no 'set') to prevent other scripts from changing the drag state directly.
    public static bool IsDragging => currentlyDraggingSpawner != null;

    // --- INSTANCE-SPECIFIC VARIABLES ---
    private bool hasSpawned = false;
    private GameObject currentItem;
    private bool imageHidden = false;

    private void Awake()
    {
        // --- AWAKE CHECKS: Check for unassigned Inspector variables ---
        if (itemPrefab == null)
        {
            Debug.LogError("NULL CHECK FAILED: 'itemPrefab' is not assigned in the Inspector for " + gameObject.name);
        }
        if (mousePositionAction == null)
        {
            Debug.LogError("NULL CHECK FAILED: 'mousePositionAction' is not assigned in the Inspector for " + gameObject.name);
        }
        if (mouseClickAction == null)
        {
            Debug.LogError("NULL CHECK FAILED: 'mouseClickAction' is not assigned in the Inspector for " + gameObject.name);
        }
        if (imageRectTransform == null)
        {
            Debug.LogError("NULL CHECK FAILED: 'imageRectTransform' is not assigned in the Inspector for " + gameObject.name);
        }
    }

    private void OnEnable()
    {
        // --- ONENABLE CHECKS ---
        if (mousePositionAction == null || mousePositionAction.action == null)
        {
            Debug.LogError("NULL CHECK FAILED: Cannot enable 'mousePositionAction' because it or its action is null.");
            return;
        }
        mousePositionAction.action.Enable();

        if (mouseClickAction == null || mouseClickAction.action == null)
        {
            Debug.LogError("NULL CHECK FAILED: Cannot enable 'mouseClickAction' because it or its action is null.");
            return;
        }
        mouseClickAction.action.Enable();
    }

    private void OnDisable()
    {
        // --- ONDISABLE CHECKS ---
        if (mousePositionAction != null && mousePositionAction.action != null)
        {
            mousePositionAction.action.Disable();
        }
        else
        {
            Debug.LogWarning("NULL CHECK WARNING: Tried to disable 'mousePositionAction' but it or its action was already null.");
        }

        if (mouseClickAction != null && mouseClickAction.action != null)
        {
            mouseClickAction.action.Disable();
        }
        else
        {
            Debug.LogWarning("NULL CHECK WARNING: Tried to disable 'mouseClickAction' but it or its action was already null.");
        }
    }

    void Update()
    {
        if (GameManager.IsPlay)
            return;

        // --- UPDATE CHECKS (Critical path) ---
        if (Camera.main == null)
        {
            Debug.LogError("NULL CHECK FAILED: Camera.main is null. Is your main camera tagged with 'MainCamera'?");
            return; // Exit Update to prevent further errors this frame
        }

        if (mousePositionAction == null || mousePositionAction.action == null)
        {
            Debug.LogError("NULL CHECK FAILED: 'mousePositionAction' or its internal action is null. Cannot read value.");
            return;
        }

        if (mouseClickAction == null || mouseClickAction.action == null)
        {
            Debug.LogError("NULL CHECK FAILED: 'mouseClickAction' or its internal action is null. Cannot read value.");
            return;
        }

        Vector3 mouseScreenPos = mousePositionAction.action.ReadValue<Vector2>();
        float spawnZ = Mathf.Abs(Camera.main.transform.position.z);
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, spawnZ));

        // --- Start Dragging ---
        if (mouseClickAction.action.WasPressedThisFrame())
        {
            // *** CHANGE: Don't start a new drag if one is already in progress.
            if (IsDragging) return;

            RectTransform selfRectTransform = transform as RectTransform;
            if (selfRectTransform == null)
            {
                Debug.LogError("NULL CHECK FAILED: The script 'ItemSpawner' is not attached to a UI object with a RectTransform. Cannot check for mouse overlap.", gameObject);
                return;
            }

            bool isMouseOverImage = RectTransformUtility.RectangleContainsScreenPoint(
                selfRectTransform,
                mouseScreenPos,
                Camera.main
            );
            if (isMouseOverImage)
            {
                // *** CHANGE: Instead of setting a local boolean, set the static reference to THIS instance.
                currentlyDraggingSpawner = this;
                hasSpawned = false;
            }
        }

        // --- Stop Dragging ---
        if (mouseClickAction.action.WasReleasedThisFrame())
        {
            // *** CHANGE: Only process release if THIS spawner is the one being dragged.
            if (currentlyDraggingSpawner == this)
            {
                // *** CHANGE: Clear the static reference to stop the drag.
                currentlyDraggingSpawner = null;
                hasSpawned = false; // Reset spawn state
                if (currentItem != null)
                {
                    Debug.Log("NULL CHECK INFO: Releasing reference to 'currentItem'.");
                }
                currentItem = null; // Release the item reference
            }
        }

        // --- Handle All Drag Logic (Spawning and Moving) ---
        // *** CHANGE: This logic now only runs for the specific spawner instance that is dragging.
        if (currentlyDraggingSpawner == this)
        {
            // 1. Check if we need to SPAWN the item.
            if (!hasSpawned)
            {
                if (imageRectTransform == null)
                {
                    Debug.LogError("NULL CHECK FAILED: 'imageRectTransform' is null. Cannot check if mouse is over the image.", gameObject);
                    currentlyDraggingSpawner = null; // Stop drag to prevent errors
                    return;
                }

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
            if (currentItem != null)
            {
                currentItem.transform.position = new Vector3(mouseWorldPos.x, mouseWorldPos.y, 0f);
            }
            else if (hasSpawned)
            {
                Debug.LogWarning("NULL CHECK WARNING: 'currentItem' is null even though 'hasSpawned' is true. Item might have been destroyed unexpectedly or failed to spawn.");
            }
        }
    }

    void SpawnItem(Vector3 position)
    {
        if (itemPrefab != null)
        {
            currentItem = Instantiate(itemPrefab, position, Quaternion.identity);
            if (currentItem == null)
            {
                Debug.LogError("NULL CHECK FAILED: Instantiate(itemPrefab) returned null. Is the 'itemPrefab' valid and not destroyed?");
                return;
            }

            RotatingObject rotatingScript = currentItem.GetComponent<RotatingObject>();
            if (rotatingScript != null)
            {
                rotatingScript.EnableDragging();
                rotatingScript.SetItemSpawner(this);
            }
            else
            {
                Debug.LogWarning("NULL CHECK WARNING: The spawned item prefab '" + itemPrefab.name + "' does not have a 'RotatingObject' component attached.", currentItem);
            }

            Debug.Log("Item spawned at: " + position);
        }
        else
        {
            Debug.LogError("NULL CHECK FAILED: 'itemPrefab' is null. Cannot spawn item.");
        }
    }

    void HideImage()
    {
        if (!imageHidden)
        {
            CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                Debug.LogWarning("NULL CHECK INFO: 'CanvasGroup' not found on '" + gameObject.name + "'. Adding one now.");
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }

            if (transform.parent == null)
            {
                Debug.LogWarning("NULL CHECK WARNING: This GameObject '" + gameObject.name + "' has no parent. Cannot use SetSiblingIndex.");
            }
            else
            {
                transform.SetSiblingIndex(transform.parent.childCount - 1);
            }

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                imageHidden = true;
            }
            else
            {
                Debug.LogError("NULL CHECK FAILED: Could not get or add a CanvasGroup component. Cannot hide image.");
            }
        }
    }

    void ShowImage()
    {
        if (imageHidden)
        {
            CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
            }
            else
            {
                Debug.LogError("NULL CHECK FAILED: 'CanvasGroup' component is missing on '" + gameObject.name + "'. Cannot show image.");
            }

            imageHidden = false;

            if (transform.parent == null)
            {
                Debug.LogWarning("NULL CHECK WARNING: This GameObject '" + gameObject.name + "' has no parent. Cannot use SetSiblingIndex.");
            }
            else
            {
                transform.SetSiblingIndex(0);
            }
        }
    }

    public void OnItemReturnedToUI(GameObject obj)
    {
        if (obj == null)
        {
            Debug.LogError("NULL CHECK FAILED: OnItemReturnedToUI was called with a null GameObject. Cannot destroy it.");
            return;
        }

        ShowImage();
        Destroy(obj);

        if (currentItem == obj)
        {
            currentItem = null;
        }
        else if (currentItem != null)
        {
            Debug.LogWarning("NULL CHECK WARNING: An object was returned to the UI, but it was not the 'currentItem' this spawner was tracking. This might be a logic issue.");
        }

        // *** CHANGE: Ensure we clear the static reference if this spawner's item is returned.
        if (currentlyDraggingSpawner == this)
        {
            currentlyDraggingSpawner = null;
        }

        hasSpawned = false;
        Debug.Log("Item returned to UI and destroyed");
    }
}