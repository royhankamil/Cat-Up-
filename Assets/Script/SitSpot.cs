using UnityEngine;
using DG.Tweening; // Make sure to import the DOTween namespace
using UnityEngine.SceneManagement;
using System.Collections; // NEW: Added to use Coroutines

// This script now checks the Player's Animator state to trigger the win condition
// and also makes the GameObject it's attached to float up and down.
public class SitSpot : MonoBehaviour
{
    [Header("Win Condition Settings")]
    // Drag your Win UI panel/GameObject here in the Unity Inspector
    [SerializeField] private GameObject winUI;
    [Tooltip("How long to wait after the sleep animation starts before triggering the win UI.")]
    [SerializeField] private float winDelay = 0.5f; // NEW: Configurable delay

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

        // --- Animation Code ---
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
            if (player.sleepCountdownDisplay != "0.0s" && !player.anim.GetBool("isSleep")) {
                NotifyManager.Instance.TriggerCustomNotify("Press S");
                if (player.anim.GetBool("isSit")) NotifyManager.Instance.TriggerCustomNotify(player.sleepCountdownDisplay);
            }


            // Step 2: Check the Player's animator to see if the "isSleep" boolean is true.
            if (player.anim.GetBool("isSleep"))
            {
                // NEW: Set hasWon flag immediately and start the delay coroutine
                // This ensures the coroutine is only started once.
                hasWon = true;
                Debug.Log("--- 'isSleep' is true! Starting win sequence... ---");
                StartCoroutine(DelayedWinSequence());
            }
        }
    }

    /// <summary>
    /// NEW: This coroutine handles the delay before showing the win UI.
    /// </summary>
    private IEnumerator DelayedWinSequence()
    {
        // Wait for the specified delay time to let the sleep animation play
        yield return new WaitForSeconds(winDelay);

        Debug.Log($"--- Delay of {winDelay}s finished. Executing win logic. ---");

        // Calculate the index for the next level.
        int currentLevelIndex = SceneManager.GetActiveScene().buildIndex;

        // Get the highest level the player has already unlocked.
        // We use a default value of 1, assuming the first level is always unlocked.
        int highestLevelUnlocked = PlayerPrefs.GetInt("Level", 1);

        // The next level to unlock is the current level's index + 1.
        // We only care about unlocking levels beyond the current one.
        int levelToUnlock = currentLevelIndex + 1;

        // Only update PlayerPrefs if the player has just unlocked a NEW, higher level.
        if (levelToUnlock > highestLevelUnlocked)
        {
            PlayerPrefs.SetInt("Level", levelToUnlock);
            Debug.Log($"New progress saved! Unlocked Level Index: {levelToUnlock}");
        }
        else
        {
            Debug.Log("Replaying a previously completed level. No new progress saved.");
        }

        // Activate Win UI and Play Sound
        if (winUI != null)
        {
            winUI.SetActive(true);
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySfx("Win");
            Debug.Log("Playing 'Win' sound effect.");
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