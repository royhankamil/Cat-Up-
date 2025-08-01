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

    [Header("Animated World Elements")]
    public Transform ground1;
    public Transform ground2;

    private bool isTransitioning = false;
    private Sequence currentSequence;

    private void Start()
    {
        DOTween.Init();
        foreach (var menu in new[] { StartMenu, MainMenu, SettingMenu, HomeMenu })
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

    public void GoToMainMenu()
    {
        if (isTransitioning) return;
        isTransitioning = true;

        AnimateButtonClick(EventSystem.current.currentSelectedGameObject.transform);

        currentSequence?.Kill();
        currentSequence = DOTween.Sequence();
        isTransitioning = false;
        currentSequence
            .AppendCallback(() =>
            {
                StartMenu.SetActive(false);
                MainMenu.SetActive(true);
            })
            .Append(MainMenu.GetComponent<CanvasGroup>().DOFade(1f, 1f).From(0f))
            // 👇 HERE ARE THE MISSING GROUND ANIMATIONS 👇
            .Join(ground1.DOLocalMove(ground1.localPosition, 2f).From(ground1.localPosition - new Vector3(0, 200, 0)).SetEase(Ease.OutCubic))
            .Join(ground2.DOLocalMove(ground2.localPosition, 1f).From(ground2.localPosition - new Vector3(0, 200, 0)).SetEase(Ease.OutCubic))
            .OnComplete(() =>
            {

            });
    }

    public void GoToSettings() // No parameter needed
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
            })
            .Append(SettingMenu.GetComponent<CanvasGroup>().DOFade(1f, 0.25f).From(0f))
            .OnComplete(() =>
            {
                isTransitioning = false;
            });
    }

    public void GoToHome() // No parameter needed
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
            })
            .Append(HomeMenu.GetComponent<CanvasGroup>().DOFade(1f, 0.25f).From(0f))
            .OnComplete(() =>
            {
                isTransitioning = false;
            });
    }

    public void Quit()
    {
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