using UnityEngine;
using DG.Tweening;

public class MainMenuManager : MonoBehaviour
{
    public GameObject StartMenu, MainMenu;
    public Transform ground1, ground2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    public void goToMainMenu()
    {
        StartMenu.SetActive(false);
        MainMenu.SetActive(true);
        MainMenu.GetComponent<CanvasGroup>().DOFade(1f, 1f).From(0f);
        AudioManager.Instance.PlaySfx("Button Click");
        ground1.transform.DOLocalMoveY(ground1.transform.localPosition.y, 2f).From(ground1.transform.localPosition.y - 200);
        ground2.transform.DOLocalMoveY(ground2.transform.localPosition.y, 1f).From(ground2.transform.localPosition.y - 200);
    }

}
