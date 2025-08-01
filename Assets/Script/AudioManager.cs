using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

// AudioManager class to control all audio in the game.
// It uses a singleton pattern to be easily accessible from anywhere.
public class AudioManager : MonoBehaviour
{
    // Static instance of the AudioManager to ensure only one exists.
    public static AudioManager Instance;

    // Arrays to hold music and sound effect clips, configured in the Inspector.
    public Sound[] musicSounds;
    public Sound[] sfxSounds;

    // Dictionaries for fast, name-based lookups at runtime.
    private Dictionary<string, Sound> musicSoundsDict = new Dictionary<string, Sound>();
    private Dictionary<string, Sound> sfxSoundsDict = new Dictionary<string, Sound>();

    // AudioSources for playing music and sound effects.
    public AudioSource musicSource;
    public AudioSource sfxSource;

    // To keep track of the fading coroutine.
    private Coroutine fadeCoroutine;

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

        // Populate the music dictionary from the inspector array for fast lookups.
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

        // Populate the SFX dictionary from the inspector array.
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

        // Load volume settings from PlayerPrefs.
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float sfxVolume = PlayerPrefs.GetFloat("SfxVolume", 1f);

        // Apply the loaded volumes.
        if (musicSource != null) musicSource.volume = musicVolume;
        if (sfxSource != null) sfxSource.volume = sfxVolume;
    }

    // This method is called when the game starts.
    private void Start()
    {
        // Example: Play background music named "Theme".
        PlayMusic("Theme");
    }

    // Plays a music track by its name using the dictionary.
    public void PlayMusic(string name)
    {
        // Find the sound in the music dictionary.
        if (!musicSoundsDict.TryGetValue(name, out Sound s))
        {
            Debug.LogWarning("Music: " + name + " not found!");
            return;
        }

        // If a fade is in progress, stop it.
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        // Set volume back to the user's preference.
        musicSource.volume = PlayerPrefs.GetFloat("MusicVolume", 1f);

        // Assign the clip and play it.
        musicSource.clip = s.clip;
        musicSource.Play();
    }

    // Stops the currently playing music with a fade out effect.
    public void StopMusic(float fadeDuration = 1.0f)
    {
        if (musicSource.isPlaying)
        {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeOut(fadeDuration));
        }
    }

    // Coroutine to gradually decrease the music volume.
    private IEnumerator FadeOut(float duration)
    {
        float startVolume = musicSource.volume;
        float timer = 0;

        while (timer < duration)
        {
            musicSource.volume = Mathf.Lerp(startVolume, 0, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }

        musicSource.volume = 0;
        musicSource.Stop();
        musicSource.volume = PlayerPrefs.GetFloat("MusicVolume", 1f);
    }

    // Plays a sound effect by its name using the dictionary.
    public void PlaySfx(string name)
    {
        // Find the sound in the SFX dictionary.
        if (!sfxSoundsDict.TryGetValue(name, out Sound s))
        {
            Debug.LogWarning("SFX: " + name + " not found!");
            return;
        }

        // Play the clip as a one-shot sound.
        sfxSource.PlayOneShot(s.clip);
    }

    // Sets the volume for the music.
    public void SetMusicVolume(float volume)
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);

        volume = Mathf.Clamp01(volume);
        musicSource.volume = volume;
        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }

    // Sets the volume for the sound effects.
    public void SetSfxVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);
        sfxSource.volume = volume;
        PlayerPrefs.SetFloat("SfxVolume", volume);
        PlayerPrefs.Save();
    }
}

// A simple helper class to organize AudioClips in the Unity Inspector.
[System.Serializable]
public class Sound
{
    public string name;      // Name of the sound (e.g., "Theme", "Jump", "Explosion").
    public AudioClip clip;   // The actual audio file.
}
