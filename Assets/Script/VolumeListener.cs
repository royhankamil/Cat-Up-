using UnityEngine;
using UnityEngine.UI;

// Attach this script to your Music and SFX sliders.
[RequireComponent(typeof(Slider))]
public class VolumeSliderSync : MonoBehaviour
{
    // Set this in the inspector for each slider to tell it which volume to sync with.
    public enum VolumeType { Music, SFX }
    public VolumeType volumeType;

    private Slider slider;

    void Start()
    {
        // Get the Slider component attached to this GameObject.
        slider = GetComponent<Slider>();

        // Set the slider's initial value from PlayerPrefs based on its type.
        if (volumeType == VolumeType.Music)
        {
            slider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        }
        else
        {
            slider.value = PlayerPrefs.GetFloat("SfxVolume", 1f);
        }

        // Add a listener to ensure the AudioManager is updated when the slider is moved.
        // This is a robust alternative to setting it up purely in the Inspector.
        if (AudioManager.Instance != null)
        {
            if (volumeType == VolumeType.Music)
            {
                slider.onValueChanged.AddListener(AudioManager.Instance.SetMusicVolume);
            }
            else
            {
                slider.onValueChanged.AddListener(AudioManager.Instance.SetSfxVolume);
            }
        }
    }
}
