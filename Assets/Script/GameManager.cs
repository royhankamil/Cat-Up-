using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static bool IsPlay { get; set; } = false;
    public GameObject winUI;
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

    public void OnReset()
    {
        IsPlay = false;
        LoadingManager.Instance.FastLoadScene(SceneManager.GetActiveScene().buildIndex);
    }

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
            rigidbodies = GameObject.FindGameObjectsWithTag("Object").Select(obj => obj.GetComponent<Rigidbody2D>()).ToList();

            foreach (var rb in rigidbodies)
            {
                if (rb != null)
                {
                    rb.bodyType = RigidbodyType2D.Dynamic;
                }
            }
        }


        // if (IsPlay)
        // {
        //     Time.timeScale = 0f;
        // }
        // else
        // {
        //     Time.timeScale = 1f;
        // }

    }

    public void OnNextLevel()
    {
        IsPlay = false;
        LoadingManager.Instance.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    
}
