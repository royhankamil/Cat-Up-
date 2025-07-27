using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static bool IsPlay { get; private set; } = false;
    [SerializeField] private TextMeshProUGUI playText;
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
