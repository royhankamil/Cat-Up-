using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class VideoSceneSwitcher : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    void Start()
    {
        // Tambahkan listener ke event saat video selesai
        videoPlayer.loopPointReached += OnVideoEnd; 
        
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        // Pindah ke scene berikutnya
        LoadingManager.Instance.LoadScene("Main_Menu");
    }
}
