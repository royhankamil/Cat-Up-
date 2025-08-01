using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public GameObject StartMenu, MainMenu;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AudioManager.Instance.PlayMusic("Waltz In C");   
    }

    public void goToMainMenu()
    {
        StartMenu.SetActive(false);
        AudioManager.Instance.StopMusic(3);
        AudioManager.Instance.PlaySfx("Button Click");
    }

}
