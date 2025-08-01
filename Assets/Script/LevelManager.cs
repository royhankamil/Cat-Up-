using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public Button[] buttons;

    private void Start() 
    {
        int level = playerprefs.GetInt("level", 1);
        for (int i = 0; i < buttons.Length; i++)
        {
            if (i < level)
            {
                buttons[i].interactable = true;
            }
            else
            {
                buttons[i].interactable = false;
            }
        }
    }

    public void loadScene(int index)
    {
        SceneManager.LoadScene(index);
    }
}
