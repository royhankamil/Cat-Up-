using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TutorialManager : MonoBehaviour
{
    [Header("Core Setup")]
    [Tooltip("Assign all your tutorial panel GameObjects here in order.")]
    public List<GameObject> tutorialPanels;

    [Tooltip("The parent object for the entire tutorial UI.")]
    public GameObject tutorialContainer;

    [Header("Navigation Buttons")]
    [Tooltip("Assign the 'Next' button.")]
    public Button nextButton;
    [Tooltip("Assign the 'Previous' button.")]
    public Button previousButton;
    [Tooltip("Assign the 'Close' button.")]
    public Button closeButton;

    [Header("Visuals")]
    [Tooltip("The color for the navigation buttons when they can't be pressed.")]
    public Color disabledButtonColor = new Color(0.7f, 0.7f, 0.7f, 0.5f);

    // To store the original "enabled" colors of the buttons
    private Color originalNextColor;
    private Color originalPrevColor;

    private int currentIndex = 0;

    // We use Awake to store the original colors before anything else happens.
    private void Awake()
    {
        if (nextButton != null)
        {
            originalNextColor = nextButton.GetComponent<Image>().color;
        }
        if (previousButton != null)
        {
            originalPrevColor = previousButton.GetComponent<Image>().color;
            previousButton.GetComponent<Image>().color = disabledButtonColor;
        }
    }

    public void OpenTutorial()
    {
        currentIndex = 0;
        if (tutorialContainer != null)
        {
            tutorialContainer.SetActive(true);
        }
        ShowPanelAtIndex(currentIndex);
    }

    public void CloseTutorial()
    {
        if (tutorialContainer != null)
        {
            tutorialContainer.SetActive(false);
        }
    }

    public void Next()
    {
        if (currentIndex < tutorialPanels.Count - 1)
        {
            currentIndex++;
            ShowPanelAtIndex(currentIndex);
        }
    }

    public void Previous()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            ShowPanelAtIndex(currentIndex);
        }
    }

    private void ShowPanelAtIndex(int index)
    {
        for (int i = 0; i < tutorialPanels.Count; i++)
        {
            tutorialPanels[i].SetActive(i == index);
        }
        // This function is called after every Next/Previous click
        UpdateButtonStates();
    }

    // This method now manually sets the color AND interactable state every time.
    private void UpdateButtonStates()
    {
        if (previousButton != null)
        {
            // Check if the button should be enabled
            if (currentIndex > 0)
            {
                previousButton.interactable = true;
                previousButton.GetComponent<Image>().color = originalPrevColor;
            }
            // Otherwise, disable it
            else
            {
                previousButton.interactable = false;
                previousButton.GetComponent<Image>().color = disabledButtonColor;
            }
        }

        if (nextButton != null)
        {
            // Check if the button should be enabled
            if (currentIndex < tutorialPanels.Count - 1)
            {
                nextButton.interactable = true;
                nextButton.GetComponent<Image>().color = originalNextColor;
            }
            // Otherwise, disable it
            else
            {
                nextButton.interactable = false;
                nextButton.GetComponent<Image>().color = disabledButtonColor;
            }
        }
    }
}