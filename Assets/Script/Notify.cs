using UnityEngine;
using TMPro; // Using TextMeshPro for better text rendering
using DG.Tweening; // Using DOTween for animations
using System.Linq;

/// <summary>
/// Manages and displays on-screen notifications.
/// Includes logic to prevent spamming of the same notification
/// and allows for custom, text-only notifications.
/// </summary>
public class NotifyManager : MonoBehaviour
{
    // --- Singleton Instance ---
    public static NotifyManager Instance { get; private set; }

    [Header("UI Reference")]
    [Tooltip("The TextMeshPro UI element that will display the notification text.")]
    [SerializeField] private TextMeshProUGUI notifyTextObject;

    [Header("Notification Data")]
    [Tooltip("Create your list of possible notifications here.")]
    [SerializeField] private Notification[] notifications;

    [Header("Animation Settings")]
    [Tooltip("How long it takes for the text to fade in and out.")]
    [SerializeField] private float fadeDuration = 0.4f;
    [Tooltip("How long the notification stays on screen at full visibility.")]
    [SerializeField] private float holdDuration = 1.5f;
    [Tooltip("How far the text moves vertically during its animation.")]
    [SerializeField] private float verticalMoveDistance = 50f;

    private Vector2 initialPosition;
    private Sequence currentAnimation;

    // To track the currently active notification
    private string currentNotificationName;
    private const string customNotificationID = "[CUSTOM]"; // A constant to identify custom notifications

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        if (notifyTextObject == null)
        {
            Debug.LogError("Notify Text Object is not assigned in the inspector!", this);
            enabled = false;
            return;
        }

        initialPosition = notifyTextObject.rectTransform.anchoredPosition;
        notifyTextObject.alpha = 0f;
        notifyTextObject.gameObject.SetActive(false);
    }

    /// <summary>
    /// Triggers a pre-defined notification with a vertical motion animation.
    /// If a different notification is playing, it will be interrupted.
    /// If the same notification is playing, this call will be ignored.
    /// </summary>
    public void TriggerNotify(string notificationName, bool withSound = true)
    {
        // Logic to handle spamming vs. new notifications
        if (currentAnimation != null && currentAnimation.IsActive())
        {
            if (currentNotificationName == notificationName)
            {
                // The same notification is already playing, so ignore this call.
                return;
            }
            // A different notification is playing, so kill it before starting the new one.
            currentAnimation.Kill();
        }

        Notification notification = notifications.FirstOrDefault(n => n.notificationName == notificationName);

        if (notification == null)
        {
            Debug.LogWarning($"Notification with name '{notificationName}' not found.");
            return;
        }

        // Prepare and play the animation
        notifyTextObject.gameObject.SetActive(true);
        notifyTextObject.text = notification.notificationText;
        notifyTextObject.alpha = 0f;
        notifyTextObject.rectTransform.anchoredPosition = initialPosition + Vector2.up * verticalMoveDistance;

        currentNotificationName = notificationName; // Track the current notification

        currentAnimation = DOTween.Sequence();
        currentAnimation.Append(notifyTextObject.DOFade(1f, fadeDuration));
        currentAnimation.Join(notifyTextObject.rectTransform.DOAnchorPosY(initialPosition.y, fadeDuration).SetEase(Ease.OutQuad));
        currentAnimation.AppendInterval(holdDuration);
        currentAnimation.Append(notifyTextObject.DOFade(0f, fadeDuration));
        currentAnimation.Join(notifyTextObject.rectTransform.DOAnchorPosY(initialPosition.y - verticalMoveDistance, fadeDuration).SetEase(Ease.InQuad));

        // Use OnKill and OnComplete to clear the tracking variable
        currentAnimation.OnComplete(() =>
        {
            notifyTextObject.gameObject.SetActive(false);
            notifyTextObject.rectTransform.anchoredPosition = initialPosition;
            currentNotificationName = null;
        });
        currentAnimation.OnKill(() => { currentNotificationName = null; });

        if (withSound) { AudioManager.Instance.PlaySfx("Notification"); }
    }

    /// <summary>
    /// Triggers a custom notification with a simple fade animation.
    /// If another custom notification is already active, it updates the text and resets the timer.
    /// Otherwise, it interrupts any current notification and plays the new one.
    /// </summary>
    /// <param name="message">The custom text to display.</param>
    public void TriggerCustomNotify(string message)
    {
        // Check if a custom notification is already visible on screen.
        bool isCustomNotifyAlreadyActive = currentAnimation != null && currentAnimation.IsActive() && currentNotificationName == customNotificationID;

        // Always kill any previously running animation.
        // This ensures timers are reset correctly.
        if (currentAnimation != null && currentAnimation.IsActive())
        {
            currentAnimation.Kill();
        }

        // --- Prepare and play the new animation ---
        notifyTextObject.gameObject.SetActive(true);
        notifyTextObject.text = message;
        notifyTextObject.rectTransform.anchoredPosition = initialPosition; // Ensure position is centered
        currentNotificationName = customNotificationID; // Track that a custom notification is running

        currentAnimation = DOTween.Sequence();

        // If a custom notification was NOT already active, we need to fade the text in.
        // If it was, the text is already visible, so we skip the fade-in to make the change seamless.
        if (!isCustomNotifyAlreadyActive)
        {
            notifyTextObject.alpha = 0f;
            currentAnimation.Append(notifyTextObject.DOFade(1f, fadeDuration));
        }
        else
        {
            // The text object is already visible, so just ensure its alpha is 1.
            notifyTextObject.alpha = 1f;
        }

        // The hold and fade-out animations are always the same.
        currentAnimation.AppendInterval(holdDuration);
        currentAnimation.Append(notifyTextObject.DOFade(0f, fadeDuration));

        // Reset state after the animation completes or is killed.
        currentAnimation.OnComplete(() =>
        {
            notifyTextObject.gameObject.SetActive(false);
            currentNotificationName = null;
        });
        currentAnimation.OnKill(() => { currentNotificationName = null; });
    }
}

// The Notification class remains the same
[System.Serializable]
public class Notification
{
    public string notificationName;
    [TextArea]
    public string notificationText;
}