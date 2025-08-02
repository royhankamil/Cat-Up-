using UnityEngine;
using UnityEngine.EventSystems; // Required to detect the clicked button
using DG.Tweening;
using UnityEditor;

/// <summary>
/// Manages the main menu UI, handling transitions between different menu panels
/// with DOTween animations. This version allows new transitions to interrupt
/// any currently running animations.
/// </summary>
public class MainMenuManager : MonoBehaviour
{
    [Header("Menus & Canvases")]
    public GameObject StartMenu;
    public GameObject MainMenu;
    public GameObject SettingMenu;
    public GameObject HomeMenu; // Assumed to be the main panel within MainMenu
    public GameObject LevelMenu; // New menu for level selection

    [Header("Animated World Elements")]
    public Transform ground1;
    public Transform ground2;

    // Tracks the last button clicked to prevent spamming the same transition.
    private GameObject lastClickedButton = null;
    // The currently active transition sequence.
    private Sequence currentSequence, seq;

    private void Start()
    {
        // Initialize DOTween
        DOTween.Init();

        // Ensure all menu GameObjects have a CanvasGroup for fading animations.
        // This is a good practice for robustness.
        foreach (var menu in new[] { StartMenu, MainMenu, SettingMenu, HomeMenu, LevelMenu })
        {
            if (menu != null && menu.GetComponent<CanvasGroup>() == null)
            {
                menu.AddComponent<CanvasGroup>();
            }
        }
    }

    /// <summary>
    /// OnDestroy is called when the object is destroyed.
    /// Kills any active DOTween sequences to prevent memory leaks or errors
    /// when the scene changes or the object is destroyed.
    /// </summary>
    private void OnDestroy()
    {
        // Kill any active DOTween sequences to prevent memory leaks.
        currentSequence?.Kill();
    }

    /// <summary>
    /// Plays a punch scale animation on a transform to provide button click feedback.
    /// </summary>
    /// <param name="buttonTransform">The transform of the button to animate.</param>
    private void AnimateButtonClick(Transform buttonTransform)
    {
        // Example of playing a sound effect. Uncomment if you have an AudioManager.
        // AudioManager.Instance.PlaySfx("Button Click");

        buttonTransform.DOPunchScale(
            punch: new Vector3(0.15f, 0.15f, 0.15f),
            duration: 0.3f,
            vibrato: 5,
            elasticity: 1);
    }

    // --- Public Methods for UI Button OnClick Events ---

    /// <summary>
    /// Transitions from the Start Menu to the Main Menu.
    /// </summary>
    public void GoToMainMenu()
    {
        GameObject clickedButton = EventSystem.current.currentSelectedGameObject;
        if (lastClickedButton == clickedButton) return;
        lastClickedButton = clickedButton;

        AnimateButtonClick(clickedButton.transform);

        // Kill any previously running sequence before starting a new one.
        seq?.Kill();
        seq = DOTween.Sequence();

        seq
            .AppendCallback(() =>
            {
                StartMenu.SetActive(false);
                MainMenu.SetActive(true);
            })
            .Append(MainMenu.GetComponent<CanvasGroup>().DOFade(1f, 1f).From(0f))
            // Animate the ground elements moving into view
            .Join(ground1.DOLocalMove(ground1.localPosition, 2f).From(ground1.localPosition - new Vector3(0, 200, 0)).SetEase(Ease.OutCubic))
            .Join(ground2.DOLocalMove(ground2.localPosition, 1f).From(ground2.localPosition - new Vector3(0, 200, 0)).SetEase(Ease.OutCubic))
            .OnComplete(() =>
            {
                lastClickedButton = null;
            });
    }

    /// <summary>
    /// Transitions to the Level Selection Menu.
    /// </summary>
    public void GoToLevelMenu()
    {
        GameObject clickedButton = EventSystem.current.currentSelectedGameObject;
        if (lastClickedButton == clickedButton) return;
        lastClickedButton = clickedButton;

        AnimateButtonClick(clickedButton.transform);

        // Kill any previously running sequence before starting a new one.
        currentSequence?.Kill();
        currentSequence = DOTween.Sequence();

        currentSequence
            .Append(StartMenu.GetComponent<CanvasGroup>().DOFade(0f, 0.25f))
            .Join(HomeMenu.GetComponent<CanvasGroup>().DOFade(0f, 0.25f)) // Fade out home menu as well
            .AppendCallback(() =>
            {
                StartMenu.SetActive(false);
                HomeMenu.SetActive(false);
                LevelMenu.SetActive(true);
                LevelMenu.GetComponent<CanvasGroup>().alpha = 0; // Ensure it starts transparent
            })
            .Append(LevelMenu.GetComponent<CanvasGroup>().DOFade(1f, 0.25f))
            .OnComplete(() =>
            {
                lastClickedButton = null;
            });
    }

    /// <summary>
    /// Transitions from the Level Menu back to the Home Menu.
    /// </summary>
    public void GoToHomeFromLevel()
    {
        GameObject clickedButton = EventSystem.current.currentSelectedGameObject;
        if (lastClickedButton == clickedButton) return;
        lastClickedButton = clickedButton;

        AnimateButtonClick(clickedButton.transform);

        // Kill any previously running sequence before starting a new one.
        currentSequence?.Kill();
        currentSequence = DOTween.Sequence();

        currentSequence
            .Append(LevelMenu.GetComponent<CanvasGroup>().DOFade(0f, 0.25f))
            .AppendCallback(() =>
            {
                LevelMenu.SetActive(false);
                HomeMenu.SetActive(true);
                HomeMenu.GetComponent<CanvasGroup>().alpha = 0;
            })
            .Append(HomeMenu.GetComponent<CanvasGroup>().DOFade(1f, 0.25f))
            .OnComplete(() =>
            {
                lastClickedButton = null;
            });
    }


    /// <summary>
    /// Transitions from the Home Menu to the Settings Menu.
    /// </summary>
    public void GoToSettings()
    {
        GameObject clickedButton = EventSystem.current.currentSelectedGameObject;
        if (lastClickedButton == clickedButton) return;
        lastClickedButton = clickedButton;

        AnimateButtonClick(clickedButton.transform);

        // Kill any previously running sequence before starting a new one.
        currentSequence?.Kill();
        currentSequence = DOTween.Sequence();

        currentSequence
            .Append(HomeMenu.GetComponent<CanvasGroup>().DOFade(0f, 0.25f))
            .AppendCallback(() =>
            {
                HomeMenu.SetActive(false);
                SettingMenu.SetActive(true);
                SettingMenu.GetComponent<CanvasGroup>().alpha = 0;
            })
            .Append(SettingMenu.GetComponent<CanvasGroup>().DOFade(1f, 0.25f))
            .OnComplete(() =>
            {
                lastClickedButton = null;
            });
    }

    /// <summary>
    /// Transitions from the Settings Menu back to the Home Menu.
    /// </summary>
    public void GoToHome()
    {
        GameObject clickedButton = EventSystem.current.currentSelectedGameObject;
        if (lastClickedButton == clickedButton) return;
        lastClickedButton = clickedButton;

        AnimateButtonClick(clickedButton.transform);

        // Kill any previously running sequence before starting a new one.
        currentSequence?.Kill();
        currentSequence = DOTween.Sequence();

        currentSequence
            .Append(SettingMenu.GetComponent<CanvasGroup>().DOFade(0f, 0.25f))
            .AppendCallback(() =>
            {
                SettingMenu.SetActive(false);
                HomeMenu.SetActive(true);
                HomeMenu.GetComponent<CanvasGroup>().alpha = 0;
            })
            .Append(HomeMenu.GetComponent<CanvasGroup>().DOFade(1f, 0.25f))
            .OnComplete(() =>
            {
                lastClickedButton = null;
            });
    }

    /// <summary>
    /// Loads a level using the LoadingManager.
    /// </summary>
    /// <param name="levelIndex">The build index of the level to load.</param>
    public void StartLevel(int levelIndex)
    {
        // Prevent starting a level load if a menu transition is happening.
        if (lastClickedButton != null) return;

        AnimateButtonClick(EventSystem.current.currentSelectedGameObject.transform);

        // NOTE: The following line is commented out because the LoadingManager script
        // was not provided. Uncomment it if you have this class in your project.
        // LoadingManager.Instance.LoadScene(levelIndex);
    }

    /// <summary>
    /// Quits the application or stops play mode in the editor.
    /// </summary>
    public void Quit(Transform transform)
    {
        AnimateButtonClick(transform);

#if UNITY_EDITOR
        // This code will only run in the Unity Editor
        EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
        // This code will only run in a built application
        Application.Quit();
#endif
    }
}
