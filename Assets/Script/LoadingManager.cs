using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene management
using TMPro;                       // Required for TextMeshPro
using DG.Tweening;                 // Required for DOTween
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

    [Tooltip("The minimum time the loading screen will be displayed, in seconds.")]
    public float minLoadingTime = 4.5f;

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
            if (canvasGroup == null)
            {
                // Add a CanvasGroup if it's missing, as it's essential for fading.
                canvasGroup = loadingScreenPanel.AddComponent<CanvasGroup>();
            }
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
    /// Public method to trigger the loading of a new scene by its name.
    /// The loading screen will be displayed during the operation.
    /// Example usage: LoadingManager.Instance.LoadScene("Level01");
    /// </summary>
    /// <param name="sceneName">The name of the scene to load.</param>
    public void LoadScene(string sceneName)
    {
        // Ensure we don't start a new loading sequence if one is already running.
        if (loadingScreenPanel.activeInHierarchy)
        {
            return;
        }
        StartCoroutine(DoLoadingSequence(sceneName));
    }

    /// <summary>
    /// Public method to trigger the loading of a new scene by its build index.
    /// The loading screen will be displayed during the operation.
    /// Example usage: LoadingManager.Instance.LoadScene(1);
    /// </summary>
    /// <param name="sceneIndex">The build index of the scene to load.</param>
    public void LoadScene(int sceneIndex)
    {
        // Ensure we don't start a new loading sequence if one is already running.
        if (loadingScreenPanel.activeInHierarchy)
        {
            return;
        }
        StartCoroutine(DoLoadingSequence(sceneIndex));
    }

    /// <summary>
    /// Coroutine that handles the entire loading process for a scene name.
    /// </summary>
    /// <param name="sceneName">The name of the scene to load.</param>
    private IEnumerator DoLoadingSequence(string sceneName)
    {
        // --- 1. Activate and Fade In ---
        yield return StartCoroutine(FadeInLoadingScreen());

        // --- 2. Load Scene Asynchronously and wait for completion ---
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        asyncOperation.allowSceneActivation = false;
        yield return StartCoroutine(WaitForLoadCompletion(asyncOperation));
    }

    /// <summary>
    /// Coroutine that handles the entire loading process for a scene index.
    /// </summary>
    /// <param name="sceneIndex">The index of the scene to load.</param>
    private IEnumerator DoLoadingSequence(int sceneIndex)
    {
        // --- 1. Activate and Fade In ---
        yield return StartCoroutine(FadeInLoadingScreen());

        // --- 2. Load Scene Asynchronously and wait for completion ---
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneIndex);
        asyncOperation.allowSceneActivation = false;
        yield return StartCoroutine(WaitForLoadCompletion(asyncOperation));
    }

    /// <summary>
    /// Handles activating the UI and fading it in.
    /// </summary>
    private IEnumerator FadeInLoadingScreen()
    {
        loadingScreenPanel.SetActive(true);

        if (loadingText != null && loadingMessages.Length > 0)
        {
            loadingText.text = loadingMessages[currentMessageIndex];
            currentMessageIndex = (currentMessageIndex + 1) % loadingMessages.Length;
        }

        canvasGroup.DOFade(1f, fadeDuration);
        yield return new WaitForSeconds(fadeDuration);
    }

    /// <summary>
    /// Waits for the async operation to be ready, ensures minimum time has passed,
    /// activates the new scene, and then fades out the loading screen.
    /// </summary>
    /// <param name="asyncOperation">The scene loading operation.</param>
    private IEnumerator WaitForLoadCompletion(AsyncOperation asyncOperation)
    {
        float loadStartTime = Time.time;

        // Wait until the scene is almost fully loaded (progress hits 0.9f).
        while (asyncOperation.progress < 0.9f)
        {
            yield return null;
        }

        // Calculate how long the load took.
        float loadTime = Time.time - loadStartTime;

        // If the load was faster than our minimum display time, wait for the remainder.
        if (loadTime < minLoadingTime)
        {
            yield return new WaitForSeconds(minLoadingTime - loadTime);
        }

        // Activate the new scene. It will now be visible behind the loading screen.
        asyncOperation.allowSceneActivation = true;

        // Wait for the scene activation to fully complete.
        while (!asyncOperation.isDone)
        {
            yield return null;
        }

        // Now that the new scene is active, fade out the loading screen to reveal it.
        canvasGroup.DOFade(0f, fadeDuration);
        yield return new WaitForSeconds(fadeDuration);

        // Deactivate the panel after it's fully hidden.
        loadingScreenPanel.SetActive(false);
    }
}