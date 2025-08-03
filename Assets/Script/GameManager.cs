using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // <-- Make sure you have this line

public class GameManager : MonoBehaviour
{
    public static bool IsPlay { get; set; } = false;
    public Button playButton;
    public GameObject winUI;
    private GameObject[] OutObjects;
    // private List<Vector3> positionsObject = new List<Vector3>();
    // private List<Quaternion> rotationsObject = new List<Quaternion>();
    // [SerializeField] private TextMeshProUGUI playText;
    // [SerializeField] private Sprite playSprite, resetSprite;
    // [SerializeField] private Image playImage;

    private List<Rigidbody2D> rigidbodies = new List<Rigidbody2D>();


    public void win()
    {
        winUI.SetActive(true); // Reset sit trigger after use
        PlayerPrefs.SetInt("Level", SceneManager.GetActiveScene().buildIndex + 1);

    }

    public void Exit()
    {
        IsPlay = false;
        LoadingManager.Instance.LoadScene(1);
    }

    public void OnRestartBack()
    {
        // IsPlay = false;
        // // playButton.gameObject.SetActive(true);    

        // for (short i = 0; i < OutObjects.Length; i++)
        // {
        //     OutObjects[i].GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        //     OutObjects[i].transform.position = positionsObject[i];
        //     OutObjects[i].transform.rotation = rotationsObject[i];
        // }

        // playButton.gameObject.SetActive(true);

        OnReset();
    }

    public void OnReset()
    {
        IsPlay = false;
        LoadingManager.Instance.FastLoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Modified method to accept the button that was pressed
    public void OnPlayPress()
    {
        IsPlay = !IsPlay;
        Debug.Log("IsPlay: " + IsPlay);
        // playImage.sprite = IsPlay ? resetSprite : playSprite;
        if (!IsPlay)
        {
            LoadingManager.Instance.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            // Disable the button as soon as play mode starts
            if (playButton != null)
            {
                playButton.gameObject.SetActive(false);
            }

            OutObjects = GameObject.FindGameObjectsWithTag("Object");
            rigidbodies = OutObjects.Select(obj => obj.GetComponent<Rigidbody2D>()).ToList();

            // for (short i = 0; i < OutObjects.Length; i++)
            // {
            //     Transform objTransform = OutObjects[i].transform;
            //     positionsObject.Add(objTransform.position);
            //     rotationsObject.Add(objTransform.rotation);
            // }

            foreach (var rb in rigidbodies)
            {
                if (rb != null)
                {
                    rb.bodyType = RigidbodyType2D.Dynamic;
                }
            }
        }
    }

    public void OnNextLevel()
    {
        IsPlay = false;
        LoadingManager.Instance.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}