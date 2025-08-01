using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public GameObject StartMenu, MainMenu;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    public void goToMainMenu()
    {
        StartMenu.SetActive(false);
        AudioManager.Instance.PlaySfx("Button Click");
    }

}
