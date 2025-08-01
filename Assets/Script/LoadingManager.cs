using UnityEngine;
using TMPro; // Required for TextMeshPro
using DG.Tweening; // Required for DOTween
using System.Collections;

// Place this script on a persistent manager GameObject.
public class LoadingManager : MonoBehaviour
{
    // Singleton instance to be accessible from anywhere.
    public static LoadingManager Instance;

    [Header("UI References")]
    [Tooltip("The parent panel of the loading screen, which contains the CanvasGroup.")]
    public GameObject loadingScreenPanel;
    [Tooltip("The TextMeshProUGUI element to display loading messages.")]
    public TextMeshProUGUI loadingText;

    [Header("Loading Configuration")]
    [Tooltip("An array of messages to display in sequence during loading.")]
    public string[] loadingMessages;

    [Tooltip("The duration of the fade-in and fade-out animations.")]
    public float fadeDuration = 0.5f;

    private CanvasGroup canvasGroup;
    // Index to track the current message in the sequence.
    private int currentMessageIndex = 0;

    private void Awake()
    {
        // --- Singleton and DontDestroyOnLoad Pattern ---
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // If another instance already exists, destroy this one.
            Destroy(gameObject);
            return;
        }

        // Get the CanvasGroup from the assigned child panel.
        if (loadingScreenPanel != null)
        {
            canvasGroup = loadingScreenPanel.GetComponent<CanvasGroup>();
        }
        else
        {
            Debug.LogError("Loading Screen Panel is not assigned in the LoadingManager Inspector!");
            return;
        }

        // Start with the loading screen hidden and the panel inactive.
        canvasGroup.alpha = 0;
        loadingScreenPanel.SetActive(false);
    }

    /// <summary>
    /// Public method to trigger the loading screen from any other script.
    /// Example usage: LoadingManager.Instance.ShowLoadingScreen();
    /// </summary>
    public void ShowLoadingScreen()
    {
        StartCoroutine(DoLoadingSequence());
    }

    private IEnumerator DoLoadingSequence()
    {
        // --- 1. Activate and Fade In ---
        loadingScreenPanel.SetActive(true); // Activate the panel first.

        // Set the next message in the sequence.
        if (loadingText != null && loadingMessages.Length > 0)
        {
            loadingText.text = loadingMessages[currentMessageIndex];
            // Increment index and loop back to 0 if it reaches the end of the array.
            currentMessageIndex = (currentMessageIndex + 1) % loadingMessages.Length;
        }

        // Animate the fade-in.
        canvasGroup.DOFade(1f, fadeDuration);

        // Wait for the fade-in to complete.
        yield return new WaitForSeconds(fadeDuration);

        // --- 2. Wait for a random duration ---
        float randomWaitTime = Random.Range(5f, 7f);
        yield return new WaitForSeconds(randomWaitTime);

        // --- 3. Fade Out and Deactivate ---
        canvasGroup.DOFade(0f, fadeDuration);

        // Wait for the fade-out to complete.
        yield return new WaitForSeconds(fadeDuration);

        // Deactivate the panel after it's fully hidden.
        loadingScreenPanel.SetActive(false);
    }
}
