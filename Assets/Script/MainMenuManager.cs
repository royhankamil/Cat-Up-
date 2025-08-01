using UnityEngine;
using UnityEngine.EventSystems; // Required to detect the clicked button
using DG.Tweening;
using UnityEditor;

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

    private bool isTransitioning = false;
    private Sequence currentSequence;

    private void Start()
    {
        DOTween.Init();
        // Loop through all menus and ensure they have a CanvasGroup component for fading.
        foreach (var menu in new[] { StartMenu, MainMenu, SettingMenu, HomeMenu, LevelMenu })
        {
            if (menu != null && menu.GetComponent<CanvasGroup>() == null)
            {
                menu.AddComponent<CanvasGroup>();
            }
        }
    }

    private void OnDestroy()
    {
        // Kill any active DOTween sequences to prevent memory leaks when the object is destroyed.
        currentSequence?.Kill();
    }

    /// <summary>
    /// Plays a punch scale animation on a transform.
    /// </summary>
    /// <param name="buttonTransform">The transform of the button to animate.</param>
    private void AnimateButtonClick(Transform buttonTransform)
    {
        // AudioManager.Instance.PlaySfx("Button Click"); // Assuming you have an AudioManager
        buttonTransform.DOPunchScale(
            punch: new Vector3(0.15f, 0.15f, 0.15f),
            duration: 0.3f,
            vibrato: 5,
            elasticity: 1);
    }

    // --- Public Methods for UI Button Events ---

    /// <summary>
    /// Transitions from the Start Menu to the Main Menu.
    /// </summary>
    public void GoToMainMenu()
    {
        if (isTransitioning) return;
        isTransitioning = true;

        AnimateButtonClick(EventSystem.current.currentSelectedGameObject.transform);

        currentSequence?.Kill();
        currentSequence = DOTween.Sequence();

        currentSequence
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
                isTransitioning = false;
            });
    }

    /// <summary>
    /// Transitions from the Start Menu to the Level Selection Menu.
    /// Assign this to your Start Button's OnClick event in the Unity Inspector.
    /// </summary>
    /// <param name="buttonTransform">The transform of the clicked button, passed from the OnClick event.</param>
    public void GoToLevelMenu(Transform buttonTransform)
    {
        if (isTransitioning) return;
        isTransitioning = true;

        // Animate the specific button that was clicked.
        AnimateButtonClick(buttonTransform);

        currentSequence?.Kill();
        currentSequence = DOTween.Sequence();

        currentSequence
            // Fade out the start menu
            .Append(StartMenu.GetComponent<CanvasGroup>().DOFade(0f, 0.25f))
            // Also fade out the home menu if it's active
            .Join(HomeMenu.GetComponent<CanvasGroup>().DOFade(0f, 0.25f))
            .AppendCallback(() =>
            {
                // Deactivate the old menus and activate the new one
                StartMenu.SetActive(false);
                HomeMenu.SetActive(false); // Explicitly deactivate it
                LevelMenu.SetActive(true);
                LevelMenu.GetComponent<CanvasGroup>().alpha = 0; // Ensure it starts transparent
            })
            // Fade in the new level menu
            .Append(LevelMenu.GetComponent<CanvasGroup>().DOFade(1f, 0.25f))
            .OnComplete(() =>
            {
                isTransitioning = false;
            });
    }

    /// <summary>
    /// Transitions from the Level Menu back to the Home/Start Menu.
    /// </summary>
    public void GoToHomeFromLevel()
    {
        if (isTransitioning) return;
        isTransitioning = true;

        AnimateButtonClick(EventSystem.current.currentSelectedGameObject.transform);

        currentSequence?.Kill();
        currentSequence = DOTween.Sequence();

        currentSequence
            // Fade out the level menu
            .Append(LevelMenu.GetComponent<CanvasGroup>().DOFade(0f, 0.25f))
            .AppendCallback(() =>
            {
                // Deactivate the level menu and activate the home menu
                LevelMenu.SetActive(false);
                HomeMenu.SetActive(true);
                HomeMenu.GetComponent<CanvasGroup>().alpha = 0;
            })
            // Fade in the home menu
            .Append(HomeMenu.GetComponent<CanvasGroup>().DOFade(1f, 0.25f))
            .OnComplete(() =>
            {
                isTransitioning = false;
            });
    }


    /// <summary>
    /// Transitions from the Home Menu to the Settings Menu.
    /// </summary>
    public void GoToSettings()
    {
        if (isTransitioning) return;
        isTransitioning = true;

        AnimateButtonClick(EventSystem.current.currentSelectedGameObject.transform);

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
                isTransitioning = false;
            });
    }

    /// <summary>
    /// Transitions from the Settings Menu back to the Home Menu.
    /// </summary>
    public void GoToHome()
    {
        if (isTransitioning) return;
        isTransitioning = true;

        AnimateButtonClick(EventSystem.current.currentSelectedGameObject.transform);

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
                isTransitioning = false;
            });
    }

    /// <summary>
    /// Quits the application or stops play mode in the editor.
    /// </summary>
    public void Quit(Transform transform)
    {
        AnimateButtonClick(transform);
        // This code will only run in a built application
#if UNITY_STANDALONE
        Application.Quit();
#endif

        // This code will only run in the Unity Editor
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }
}
