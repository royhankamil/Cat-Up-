using UnityEngine;
using DG.Tweening; // Make sure to import the DOTween namespace

// This script now checks the Player's Animator state to trigger the win condition
// and also makes the GameObject it's attached to float up and down.
public class SitSpot : MonoBehaviour
{
    [Header("Win Condition Settings")]
    // Drag your Win UI panel/GameObject here in the Unity Inspector
    [SerializeField] private GameObject winUI;

    [Header("Floating Animation Settings")]
    // How high the object will float from its starting point
    [SerializeField] private float floatDistance = 0.5f;
    // How long one cycle of the float animation (up and down) takes
    [SerializeField] private float floatDuration = 3f;


    private Player player; // A reference to the Player component
    private bool isPlayerOnSpot = false;
    private bool hasWon = false;
    private Vector3 startPos; // To store the starting position for the animation

    void Start()
    {
        Debug.Log("SitSpot script started. Waiting for Player.");
        if (winUI != null)
        {
            winUI.SetActive(false);
        }
        else
        {
            Debug.LogError("Win UI has not been assigned in the Inspector on " + gameObject.name + "!");
        }

        // --- New Animation Code ---
        // Store the starting position of the GameObject
        startPos = transform.position;
        // Start the floating animation loop
        transform.DOMoveY(startPos.y + floatDistance, floatDuration / 2)
            .SetEase(Ease.InOutSine) // Makes the movement smooth
            .SetLoops(-1, LoopType.Yoyo); // -1 makes it loop forever, Yoyo makes it go back and forth
    }

    void Update()
    {
        // Step 1: Check if the player is on the spot and we haven't already won.
        if (isPlayerOnSpot && player != null && !hasWon)
        {
            // Step 2: Check the Player's animator to see if the "isSleep" boolean is true.
            // This assumes your 'Player' script has a public Animator variable named 'anim'.
            if (player.anim.GetBool("isSleep"))
            {
                Debug.Log("--- 'isSleep' is true! WIN CONDITION MET! ---");

                hasWon = true; // Set flag to ensure this block runs only once

                // Activate Win UI
                if (winUI != null)
                {
                    winUI.SetActive(true);
                    Debug.Log("Win UI activated.");
                }

                // Play Win Sound
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlaySfx("Win");
                    Debug.Log("Playing 'Win' sound effect.");
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player has entered the SitSpot.");
            isPlayerOnSpot = true;
            // Get and store the Player component from the colliding object
            player = collision.GetComponent<Player>();
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player has exited the SitSpot.");
            isPlayerOnSpot = false;
            // Clear the player reference when they leave
            player = null;
        }
    }
}
