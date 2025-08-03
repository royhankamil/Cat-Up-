using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public Button[] buttons;

    public void levelOpened()
    {
        // Define the color for disabled buttons
        Color disabledColor;
        ColorUtility.TryParseHtmlString("#7E7E7E", out disabledColor);

        int level = PlayerPrefs.GetInt("Level", 1);
        for (int i = 0; i < buttons.Length; i++)
        {
            if (i < level)
            {
                buttons[i].interactable = true;
            }
            else
            {
                buttons[i].interactable = false;

                // Get the button's current color block
                ColorBlock cb = buttons[i].colors;
                // Set the disabled color
                cb.disabledColor = disabledColor;
                // Apply the new color block to the button
                buttons[i].colors = cb;
            }
        }
    }

    public void loadScene(int index)
    {
        // Assuming you have a LoadingManager script
        LoadingManager.Instance.LoadScene(index);
    }
}