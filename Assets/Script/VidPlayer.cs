using UnityEngine;
using UnityEngine.Video;

public class VidPlayer : MonoBehaviour
{
    [SerializeField] string videoFileName;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayVideo();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void PlayVideo()
    {
        VideoPlayer videoPlayer = GetComponent<VideoPlayer>();
        if (videoPlayer != null)
        {
            string videoPath = System.IO.Path.Combine(Application.streamingAssetsPath, videoFileName);
            Debug.Log(videoPlayer);
            videoPlayer.url = videoPath;
            videoPlayer.Play();
        }
        else
        {
            Debug.LogError("VideoPlayer component not found on " + gameObject.name);
        }
    }
}
