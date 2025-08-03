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

    private int currentIndex = 0;

    public void OpenTutorial()
    {
        // Play animation if the effector and button are assigned
        // Note: You would need to pass in the transform of the button that OPENS the tutorial.
        // For simplicity, we'll skip animating the opening button here.

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
        // Animate, then execute logic


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
        UpdateButtonStates();
    }

    private void UpdateButtonStates()
    {
        if (previousButton != null)
        {
            previousButton.interactable = (currentIndex > 0);
        }

        if (nextButton != null)
        {
            nextButton.interactable = (currentIndex < tutorialPanels.Count - 1);
        }
    }
}