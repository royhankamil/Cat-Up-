using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

/// <summary>
/// Animates a UI window with a distinct overlay and container.
/// Handles showing and hiding the UI with modern, fluid animations using DOTween.
/// The caller is responsible for managing the GameObject's active state.
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class WinUIAnimator : MonoBehaviour
{
    [Header("UI Components")]
    [Tooltip("The semi-transparent background panel that blocks raycasts.")]
    public GameObject overlayPanel;

    [Tooltip("The panel that holds the actual UI content (buttons, text, etc.).")]
    public GameObject containerPanel;

    [Header("Animation Settings")]
    [Tooltip("How long the show/hide animation takes.")]
    public float animationDuration = 0.3f;

    [Tooltip("The easing function for the show animation.")]
    public Ease showEase = Ease.OutBack;

    [Tooltip("The easing function for the hide animation.")]
    public Ease hideEase = Ease.InCubic;

    [Header("Behavior")]
    [Tooltip("If true, the Show() animation will automatically play when the GameObject is enabled.")]
    public bool showOnEnable = true;

    // Private references
    private CanvasGroup canvasGroup;
    private Transform containerTransform;
    private CanvasGroup overlayCanvasGroup;
    private CanvasGroup containerCanvasGroup;

    private Sequence currentAnimation;

    private void Awake()
    {
        // --- Get necessary components ---
        canvasGroup = GetComponent<CanvasGroup>();

        if (containerPanel == null || overlayPanel == null)
        {
            Debug.LogError("WinUIAnimator: Overlay Panel or Container Panel is not assigned in the Inspector!", this);
            return;
        }

        containerTransform = containerPanel.transform;

        // Ensure child panels have a CanvasGroup for fading
        if (!overlayPanel.TryGetComponent(out overlayCanvasGroup))
        {
            overlayCanvasGroup = overlayPanel.AddComponent<CanvasGroup>();
        }
        if (!containerPanel.TryGetComponent(out containerCanvasGroup))
        {
            containerCanvasGroup = containerPanel.AddComponent<CanvasGroup>();
        }

        // --- Initialize UI State ---
        // Start with the UI hidden, ready for the Show animation.
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    /// <summary>
    /// Called when the GameObject becomes active.
    /// </summary>
    private void OnEnable()
    {
        // If the flag is set, automatically start the show animation.
        if (showOnEnable)
        {
            Show();
        }
    }

    /// <summary>
    /// Shows the UI window with a smooth animation.
    /// Assumes the GameObject is already active.
    /// </summary>
    public void Show()
    {
        // Prevent running multiple animations
        currentAnimation?.Kill();

        // Set initial states for animation
        overlayCanvasGroup.alpha = 0;
        containerTransform.localScale = Vector3.one * 0.9f;
        containerCanvasGroup.alpha = 0;

        currentAnimation = DOTween.Sequence();

        currentAnimation
            // Set the parent group to be visible to allow the animation to play.
            .AppendCallback(() => canvasGroup.alpha = 1)
            // Animate the overlay and container simultaneously
            .Append(overlayCanvasGroup.DOFade(1f, animationDuration * 0.8f))
            .Join(containerCanvasGroup.DOFade(1f, animationDuration))
            .Join(containerTransform.DOScale(1f, animationDuration).SetEase(showEase))
            .OnComplete(() =>
            {
                // Enable interaction once the animation is complete
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            });
    }

    /// <summary>
    /// Hides the UI window with a smooth animation.
    /// The caller is responsible for disabling the GameObject afterwards if desired.
    /// </summary>
    public void Hide()
    {
        // Prevent running multiple animations
        currentAnimation?.Kill();

        // Disable interaction immediately
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        currentAnimation = DOTween.Sequence();

        currentAnimation
            // Animate the overlay and container simultaneously
            .Append(overlayCanvasGroup.DOFade(0f, animationDuration))
            .Join(containerCanvasGroup.DOFade(0f, animationDuration * 0.8f))
            .Join(containerTransform.DOScale(0.9f, animationDuration).SetEase(hideEase))
            .OnComplete(() =>
            {
                // Fully hide the parent CanvasGroup.
                // The calling script can now disable the GameObject.
                canvasGroup.alpha = 0;
            });
    }

    // --- Editor Helper ---
    [ContextMenu("Auto-Assign Components")]
    private void AutoAssignComponents()
    {
        overlayPanel = transform.Find("Overlay")?.gameObject;
        containerPanel = transform.Find("Container")?.gameObject;

        if (overlayPanel == null || containerPanel == null)
        {
            Debug.LogWarning("Could not find children named 'Overlay' and/or 'Container'. Please create and assign them manually.", this);
        }
        else
        {
            // Ensure the container has a canvas group for animations
            if (containerPanel.GetComponent<CanvasGroup>() == null)
            {
                containerPanel.AddComponent<CanvasGroup>();
            }
            Debug.Log("Successfully assigned Overlay and Container panels.", this);
        }
    }
}
