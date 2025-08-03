using UnityEngine;
using UnityEngine.EventSystems;
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
    public GameObject LevelMenu;
    public GameObject CreditMenu; // --- NEW --- Reference to the credits panel

    [Header("Animated World Elements")]
    public Transform ground1;
    public Transform ground2;

    // Tracks the last button clicked to prevent spamming the same transition.
    private GameObject lastClickedButton = null;
    // The currently active transition sequence.
    private Sequence currentSequence;

    // Variables to store the original positions
    private Vector3 ground1InitialPos;
    private Vector3 ground2InitialPos;

    private void Start()
    {
        // Initialize DOTween
        DOTween.Init();

        // Record the initial positions here
        if (ground1 != null) ground1InitialPos = ground1.localPosition;
        if (ground2 != null) ground2InitialPos = ground2.localPosition;

        // Ensure all menu GameObjects have a CanvasGroup for fading animations.
        // This is a good practice for robustness.
        // --- NEW: Added CreditMenu to the loop ---
        foreach (var menu in new[] { StartMenu, MainMenu, SettingMenu, HomeMenu, LevelMenu, CreditMenu })
        {
            if (menu != null && menu.GetComponent<CanvasGroup>() == null)
            {
                menu.AddComponent<CanvasGroup>();
            }
        }
    }

    private void OnDestroy()
    {
        currentSequence?.Kill();
    }

    public void AnimateButtonClick(Transform buttonTransform)
    {
        //AudioManager.Instance.PlaySfx("Button Click");
        buttonTransform.DOKill(true);
        buttonTransform.DOPunchScale(
            punch: new Vector3(0.15f, 0.15f, 0.15f),
            duration: 0.3f,
            vibrato: 5,
            elasticity: 1);
    }

    public void GoToMainMenu()
    {
        GameObject clickedButton = EventSystem.current.currentSelectedGameObject;
        if (lastClickedButton == clickedButton) return;
        lastClickedButton = clickedButton;

        AnimateButtonClick(clickedButton.transform);

        currentSequence?.Kill();
        currentSequence = DOTween.Sequence();

        currentSequence
            .AppendCallback(() =>
            {
                StartMenu.SetActive(false);
                MainMenu.SetActive(true);
            })
            .Append(MainMenu.GetComponent<CanvasGroup>().DOFade(1f, 0.5f).From(0f))
            // Use the stored initial positions
            .Join(ground1.DOLocalMove(ground1InitialPos, 2f).From(ground1InitialPos - new Vector3(0, 200, 0)).SetEase(Ease.OutCubic))
            .Join(ground2.DOLocalMove(ground2InitialPos, 1f).From(ground2InitialPos - new Vector3(0, 200, 0)).SetEase(Ease.OutCubic))
            .OnComplete(() =>
            {
                lastClickedButton = null;
            });
    }

    public void GoToLevelMenu()
    {
        MainMenu.GetComponent<CanvasGroup>().alpha = 1;
        GameObject clickedButton = EventSystem.current.currentSelectedGameObject;
        if (lastClickedButton == clickedButton) return;
        lastClickedButton = clickedButton;

        AnimateButtonClick(clickedButton.transform);

        currentSequence?.Kill();
        currentSequence = DOTween.Sequence();

        currentSequence
            .Append(HomeMenu.GetComponent<CanvasGroup>().DOFade(0f, 0.25f))
            .AppendCallback(() =>
            {
                HomeMenu.SetActive(false);
                LevelMenu.SetActive(true);
                LevelMenu.GetComponent<CanvasGroup>().alpha = 0;
            })
            .Append(LevelMenu.GetComponent<CanvasGroup>().DOFade(1f, 0.25f))
            .OnComplete(() =>
            {
                lastClickedButton = null;
            });
    }

    public void GoToHomeFromLevel()
    {
        GameObject clickedButton = EventSystem.current.currentSelectedGameObject;
        if (lastClickedButton == clickedButton) return;
        lastClickedButton = clickedButton;

        AnimateButtonClick(clickedButton.transform);

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

    public void GoToSettings()
    {
        MainMenu.GetComponent<CanvasGroup>().alpha = 1;
        GameObject clickedButton = EventSystem.current.currentSelectedGameObject;
        if (lastClickedButton == clickedButton) return;
        lastClickedButton = clickedButton;

        AnimateButtonClick(clickedButton.transform);

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

    public void GoToHome()
    {
        GameObject clickedButton = EventSystem.current.currentSelectedGameObject;
        if (lastClickedButton == clickedButton) return;
        lastClickedButton = clickedButton;

        AnimateButtonClick(clickedButton.transform);

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

    // --- NEW METHOD: GoToCreditScene ---
    /// <summary>
    /// Fades out the Home menu and fades in the Credit menu.
    /// </summary>
    public void GoToCreditScene()
    {
        GameObject clickedButton = EventSystem.current.currentSelectedGameObject;
        if (lastClickedButton == clickedButton) return;
        lastClickedButton = clickedButton;

        AnimateButtonClick(clickedButton.transform);

        currentSequence?.Kill();
        currentSequence = DOTween.Sequence();

        currentSequence
            .Append(HomeMenu.GetComponent<CanvasGroup>().DOFade(0f, 0.25f))
            .AppendCallback(() =>
            {
                HomeMenu.SetActive(false);
                CreditMenu.SetActive(true);
                CreditMenu.GetComponent<CanvasGroup>().alpha = 0;
            })
            .Append(CreditMenu.GetComponent<CanvasGroup>().DOFade(1f, 0.25f))
            .OnComplete(() =>
            {
                lastClickedButton = null;
            });
    }

    // --- NEW METHOD: GoToHomeFromCreditScene ---
    /// <summary>
    /// Fades out the Credit menu and fades in the Home menu.
    /// </summary>
    public void GoToHomeFromCreditScene()
    {
        GameObject clickedButton = EventSystem.current.currentSelectedGameObject;
        if (lastClickedButton == clickedButton) return;
        lastClickedButton = clickedButton;

        AnimateButtonClick(clickedButton.transform);

        currentSequence?.Kill();
        currentSequence = DOTween.Sequence();

        currentSequence
            .Append(CreditMenu.GetComponent<CanvasGroup>().DOFade(0f, 0.25f))
            .AppendCallback(() =>
            {
                CreditMenu.SetActive(false);
                HomeMenu.SetActive(true);
                HomeMenu.GetComponent<CanvasGroup>().alpha = 0;
            })
            .Append(HomeMenu.GetComponent<CanvasGroup>().DOFade(1f, 0.25f))
            .OnComplete(() =>
            {
                lastClickedButton = null;
            });
    }


    public void StartLevel(int levelIndex)
    {
        if (lastClickedButton != null) return;
        AnimateButtonClick(EventSystem.current.currentSelectedGameObject.transform);
        // LoadingManager.Instance.LoadScene(levelIndex);
    }

    /// <summary>
    /// Transitions from the main hub back to the initial Start Menu.
    /// </summary>
    public void ReturnToStartMenu()
    {
        GameObject clickedButton = EventSystem.current.currentSelectedGameObject;
        if (lastClickedButton == clickedButton) return;
        lastClickedButton = clickedButton;

        AnimateButtonClick(clickedButton.transform);

        currentSequence?.Kill();
        currentSequence = DOTween.Sequence();

        currentSequence
            .Append(MainMenu.GetComponent<CanvasGroup>().DOFade(0f, 0.5f))
            // Use the stored initial positions
            .Join(ground1.DOLocalMove(ground1InitialPos - new Vector3(0, 200, 0), 0.5f).SetEase(Ease.InCubic))
            .Join(ground2.DOLocalMove(ground2InitialPos - new Vector3(0, 200, 0), 0.5f).SetEase(Ease.InCubic))
            .AppendCallback(() =>
            {
                MainMenu.SetActive(false);
                HomeMenu.SetActive(true);
                HomeMenu.GetComponent<CanvasGroup>().alpha = 1f;
                SettingMenu.SetActive(false);
                LevelMenu.SetActive(false);
                // --- NEW: Also ensure CreditMenu is disabled on return ---
                CreditMenu.SetActive(false);
                StartMenu.SetActive(true);
                StartMenu.GetComponent<CanvasGroup>().alpha = 0;
            })
            .Append(StartMenu.GetComponent<CanvasGroup>().DOFade(1f, 0.5f))
            .OnComplete(() =>
            {
                lastClickedButton = null;
            });
    }

    /// <summary>
    /// Returns to the start menu.
    /// </summary>
    public void Quit()
    {
        ReturnToStartMenu();
    }
}