using UnityEngine;

/// <summary>
/// Manages the game's cursor. Implements a singleton pattern to be easily accessible
/// from any script.
/// </summary>
public class CursorManager : MonoBehaviour
{
    /// <summary>
    /// The static instance of the CursorManager, allowing access from anywhere.
    /// </summary>
    public static CursorManager instance;

    /// <summary>
    /// A data structure to hold information about a specific cursor type.
    /// You can edit these in the Unity Inspector.
    /// </summary>
    [System.Serializable]
    public class CursorData
    {
        [Tooltip("The name used to identify this cursor.")]
        public string name;

        [Tooltip("The texture for the cursor. Make sure its 'Texture Type' is set to 'Cursor' in the import settings.")]
        public Texture2D texture;

        [Tooltip("The offset from the top-left of the texture to use as the cursor's hotspot.")]
        public Vector2 hotspot;
    }

    [Tooltip("The list of all available cursors for the game. The first element is used as the default.")]
    public CursorData[] cursors;

    // Awake is called when the script instance is being loaded.
    private void Awake()
    {
        // --- Singleton Pattern Implementation ---
        // If an instance doesn't exist, this becomes the instance.
        if (instance == null)
        {
            instance = this;
            // Optional: Keep this object alive when loading new scenes.
            DontDestroyOnLoad(gameObject);
        }
        // If an instance already exists, destroy this duplicate.
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        // Check if there are any cursors defined in the array
        if (cursors != null && cursors.Length > 0)
        {
            // Set the first cursor in the array as the default cursor at the start of the game.
            SetCursor(cursors[0].name);
        }
        else
        {
            Debug.LogWarning("CursorManager: No cursors have been assigned in the Inspector.");
        }
    }


    /// <summary>
    /// Sets the hardware cursor to the one specified by name.
    /// </summary>
    /// <param name="cursorName">The name of the cursor to set, as defined in the 'cursors' array.</param>
    public void SetCursor(string cursorName)
    {
        // Loop through our array of cursor data
        foreach (var cursor in cursors)
        {
            // If we find a cursor with the matching name
            if (cursor.name == cursorName)
            {
                // Set the system cursor
                Cursor.SetCursor(cursor.texture, cursor.hotspot, CursorMode.Auto);
                // Exit the method
                return;
            }
        }

        // If the loop finishes without finding a matching name, log a warning.
        Debug.LogWarning("CursorManager: Cursor with name '" + cursorName + "' not found!");
    }

    /// <summary>
    /// Resets the cursor to the default system arrow.
    /// </summary>
    public void ResetToDefault()
    {
        // Passing null to SetCursor reverts it to the default hardware cursor.
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}
