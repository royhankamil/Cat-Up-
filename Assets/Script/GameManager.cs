using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static bool IsPlay { get; private set; } = false;
    [SerializeField] private TextMeshProUGUI playText;
    private List<Rigidbody2D> rigidbodies = new List<Rigidbody2D>();
    void Start()
    {

    }

    void Update()
    {

    }

    public void OnPlayPress()
    {
        IsPlay = !IsPlay;
        playText.text = IsPlay ? "Reset" : "Play";
        if (!IsPlay)
        {
            SceneManager.LoadScene(0);
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
    
}
