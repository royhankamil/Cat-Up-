using UnityEngine;

// This script now checks the Player's Animator state to trigger the win condition.
public class SitSpot : MonoBehaviour
{
    // Drag your Win UI panel/GameObject here in the Unity Inspector
    [SerializeField] private GameObject winUI;

    private Player player; // A reference to the Player component
    private bool isPlayerOnSpot = false;
    private bool hasWon = false;

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