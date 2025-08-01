using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;

// AudioManager class to control all audio in the game.
// It uses a singleton pattern to be easily accessible from anywhere.
public class AudioManager : MonoBehaviour
{
    // Static instance of the AudioManager to ensure only one exists.
    public static AudioManager Instance;

    // Arrays to hold sound clips, configured in the Inspector.
    public Sound[] musicSounds;
    public Sound[] sfxSounds;

    // Array to configure which music plays on which scene.
    public SceneMusic[] sceneMusicSettings;

    // Dictionaries for fast, name-based lookups at runtime.
    private Dictionary<string, Sound> musicSoundsDict = new Dictionary<string, Sound>();
    private Dictionary<string, Sound> sfxSoundsDict = new Dictionary<string, Sound>();

    // AudioSources for playing two layers of music and sound effects.
    public AudioSource musicSource1;
    public AudioSource musicSource2;
    public AudioSource sfxSource;

    // To keep track of fading coroutines for each music source.
    private Coroutine fadeCoroutine1;
    private Coroutine fadeCoroutine2;

    // This method is called when the script instance is being loaded.
    private void Awake()
    {
        // Singleton pattern implementation.
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Populate the music dictionary for fast lookups.
        foreach (Sound s in musicSounds)
        {
            if (!musicSoundsDict.ContainsKey(s.name))
            {
                musicSoundsDict.Add(s.name, s);
            }
            else
            {
                Debug.LogWarning("Duplicate music sound name found: " + s.name);
            }
        }

        // Populate the SFX dictionary.
        foreach (Sound s in sfxSounds)
        {
            if (!sfxSoundsDict.ContainsKey(s.name))
            {
                sfxSoundsDict.Add(s.name, s);
            }
            else
            {
                Debug.LogWarning("Duplicate SFX sound name found: " + s.name);
            }
        }

        // Load and apply volume settings.
        ApplyVolumeSettings();
    }

    private void OnEnable()
    {
        // Subscribe to the sceneLoaded event to handle music changes.
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // Unsubscribe to prevent memory leaks.
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Called automatically when a new scene is loaded.
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string currentSceneName = scene.name;
        // Search for a music setting that matches the loaded scene.
        foreach (var setting in sceneMusicSettings)
        {
            if (setting.sceneName == currentSceneName)
            {
                // Stop any currently playing music and play the new track.
                StopMusic(0.5f); // Optional fade out of old music
                PlayMusic(setting.musicName, 1); // Play new music on layer 1
                return; // Exit after finding the correct scene setting.
            }
        }
    }

    // Plays a music track by its name on a specific layer.
    public void PlayMusic(string name, int layer = 1)
    {
        if (!musicSoundsDict.TryGetValue(name, out Sound s))
        {
            Debug.LogWarning("Music: " + name + " not found!");
            return;
        }

        AudioSource targetSource = (layer == 1) ? musicSource1 : musicSource2;
        Coroutine activeFade = (layer == 1) ? fadeCoroutine1 : fadeCoroutine2;

        // Do nothing if the target AudioSource is not assigned in the inspector.
        if (targetSource == null)
        {
            Debug.LogWarning("Music layer " + layer + " is not assigned in the AudioManager Inspector.");
            return;
        }

        if (activeFade != null)
        {
            StopCoroutine(activeFade);
        }

        targetSource.volume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        targetSource.clip = s.clip;
        targetSource.Play();
    }

    // Stops all music with a fade out effect.
    public void StopMusic(float fadeDuration = 1.0f)
    {
        if (musicSource1 != null && musicSource1.isPlaying)
        {
            if (fadeCoroutine1 != null) StopCoroutine(fadeCoroutine1);
            fadeCoroutine1 = StartCoroutine(FadeOut(musicSource1, fadeDuration));
        }
        if (musicSource2 != null && musicSource2.isPlaying)
        {
            if (fadeCoroutine2 != null) StopCoroutine(fadeCoroutine2);
            fadeCoroutine2 = StartCoroutine(FadeOut(musicSource2, fadeDuration));
        }
    }

    // Coroutine to gradually decrease the volume of a specific AudioSource.
    private IEnumerator FadeOut(AudioSource source, float duration)
    {
        float startVolume = source.volume;
        float timer = 0;

        while (timer < duration)
        {
            source.volume = Mathf.Lerp(startVolume, 0, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }

        source.volume = 0;
        source.Stop();
        // Reset volume for next playback.
        source.volume = PlayerPrefs.GetFloat("MusicVolume", 1f);
    }

    // Plays a sound effect by its name.
    public void PlaySfx(string name)
    {
        if (sfxSource == null) return; // Don't play if source is not assigned.
        if (!sfxSoundsDict.TryGetValue(name, out Sound s))
        {
            Debug.LogWarning("SFX: " + name + " not found!");
            return;
        }
        sfxSource.PlayOneShot(s.clip);
    }

    // Sets the volume for both music layers.
    public void SetMusicVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);

        if (musicSource1 != null) musicSource1.volume = volume;
        if (musicSource2 != null) musicSource2.volume = volume;

        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }

    // Sets the volume for sound effects.
    public void SetSfxVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);
        if (sfxSource != null) sfxSource.volume = volume;
        PlayerPrefs.SetFloat("SfxVolume", volume);
        PlayerPrefs.Save();
    }

    private void ApplyVolumeSettings()
    {
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float sfxVolume = PlayerPrefs.GetFloat("SfxVolume", 1f);

        if (musicSource1 != null) musicSource1.volume = musicVolume;
        if (musicSource2 != null) musicSource2.volume = musicVolume;
        if (sfxSource != null) sfxSource.volume = sfxVolume;
    }
}

// A simple helper class to organize AudioClips.
[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
}

// A helper class to link a scene name to a music track name.
[System.Serializable]
public class SceneMusic
{
    public string sceneName;
    public string musicName;
}
