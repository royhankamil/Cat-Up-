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

    [Header("Sound Arrays")]
    public Sound[] musicSounds;
    public Sound[] sfxSounds;

    [Header("Scene Music Configuration")]
    // Array to configure which music plays on which scene and on which layer.
    public SceneMusic[] sceneMusicSettings;

    [Header("Audio Sources")]
    // AudioSources for playing two layers of music and sound effects.
    public AudioSource musicSource1;
    public AudioSource musicSource2;
    public AudioSource sfxSource;

    // Dictionaries for fast, name-based lookups at runtime.
    private Dictionary<string, Sound> musicSoundsDict = new Dictionary<string, Sound>();
    private Dictionary<string, Sound> sfxSoundsDict = new Dictionary<string, Sound>();

    // To keep track of fading coroutines for each music source.
    private Coroutine fadeCoroutine1;
    private Coroutine fadeCoroutine2;

    private void Awake()
    {
        PlayerPrefs.GetFloat("MusicVolume", 1f);
        PlayerPrefs.GetFloat("SfxVolume", 1f);
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
                musicSoundsDict.Add(s.name, s);
            else
                Debug.LogWarning("Duplicate music sound name found: " + s.name);
        }

        // Populate the SFX dictionary.
        foreach (Sound s in sfxSounds)
        {
            if (!sfxSoundsDict.ContainsKey(s.name))
                sfxSoundsDict.Add(s.name, s);
            else
                Debug.LogWarning("Duplicate SFX sound name found: " + s.name);
        }

        // Load and apply volume settings.
        ApplyVolumeSettings();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Called automatically when a new scene is loaded.
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string currentSceneName = scene.name;
        // Search for all music settings that match the loaded scene.
        foreach (var setting in sceneMusicSettings)
        {
            if (setting.sceneName == currentSceneName)
            {
                // Play the music on its designated layer with a crossfade.
                // The loop continues to find all entries for the current scene.
                PlayMusic(setting.musicName, setting.layer, 1.0f);
            }
        }
    }

    /// <summary>
    /// Plays a music track by name on a specific layer, with an optional crossfade.
    /// </summary>
    /// <param name="name">The name of the music track.</param>
    /// <param name="layer">The layer to play on (1 or 2).</param>
    /// <param name="fadeDuration">The duration of the crossfade.</param>
    public void PlayMusic(string name, int layer = 1, float fadeDuration = 1.0f)
    {
        if (!musicSoundsDict.TryGetValue(name, out Sound s))
        {
            Debug.LogWarning("Music: " + name + " not found!");
            return;
        }

        // For music, we expect at least one clip. We'll use the first one.
        if (s.clips == null || s.clips.Length == 0 || s.clips[0] == null)
        {
            Debug.LogWarning("Music sound '" + name + "' has no valid clip assigned.");
            return;
        }
        AudioClip musicClip = s.clips[0];

        AudioSource targetSource = (layer == 1) ? musicSource1 : musicSource2;
        if (targetSource == null)
        {
            Debug.LogWarning("Music layer " + layer + " is not assigned in the AudioManager Inspector.");
            return;
        }

        // If the same track is already playing on this layer, do nothing.
        if (targetSource.isPlaying && targetSource.clip == musicClip)
        {
            return;
        }

        // Stop any previous fade coroutine running on this layer.
        if (layer == 1 && fadeCoroutine1 != null) StopCoroutine(fadeCoroutine1);
        if (layer == 2 && fadeCoroutine2 != null) StopCoroutine(fadeCoroutine2);

        // Start the crossfade with the selected clip and store a reference to the new coroutine.
        Coroutine newFade = StartCoroutine(Crossfade(targetSource, musicClip, fadeDuration));
        if (layer == 1)
            fadeCoroutine1 = newFade;
        else
            fadeCoroutine2 = newFade;
    }

    /// <summary>
    /// Stops the music on a specific layer with a fade out.
    /// </summary>
    public void StopMusicOnLayer(int layer, float fadeDuration = 1.0f)
    {
        AudioSource sourceToStop = (layer == 1) ? musicSource1 : musicSource2;
        if (sourceToStop != null && sourceToStop.isPlaying)
        {
            if (layer == 1 && fadeCoroutine1 != null) StopCoroutine(fadeCoroutine1);
            if (layer == 2 && fadeCoroutine2 != null) StopCoroutine(fadeCoroutine2);

            var fade = StartCoroutine(FadeOut(sourceToStop, fadeDuration));
            if (layer == 1) fadeCoroutine1 = fade;
            else fadeCoroutine2 = fade;
        }
    }

    /// <summary>
    /// Stops all currently playing music with a fade out.
    /// </summary>
    public void StopAllMusic(float fadeDuration = 1.0f)
    {
        StopMusicOnLayer(1, fadeDuration);
        StopMusicOnLayer(2, fadeDuration);
    }

    private IEnumerator Crossfade(AudioSource source, AudioClip newClip, float duration)
    {
        float masterVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);

        // Determine the correct target volume based on which source is being used.
        float targetVolume = (source == musicSource2) ? masterVolume / 2f : masterVolume;

        // If the source is already playing, fade it out first.
        if (source.isPlaying)
        {
            yield return StartCoroutine(FadeOut(source, duration / 2));
        }

        // Set up the new clip and fade it in to the correct target volume.
        source.clip = newClip;
        source.Play();
        float timer = 0;
        while (timer < duration / 2)
        {
            source.volume = Mathf.Lerp(0, targetVolume, timer / (duration / 2));
            timer += Time.deltaTime;
            yield return null;
        }
        source.volume = targetVolume;
    }

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
        source.Stop();
        source.clip = null;
    }

    // --- MODIFIED START ---
    /// <summary>
    /// Plays an SFX by name, respecting its chance and individual volume settings.
    /// </summary>
    public void PlaySfx(string name)
    {
        if (sfxSource == null) return;
        if (!sfxSoundsDict.TryGetValue(name, out Sound s))
        {
            Debug.LogWarning("SFX: " + name + " not found!");
            return;
        }

        // Check if the sound should play based on its chance property.
        if (UnityEngine.Random.value > s.chance)
        {
            return;
        }

        // Check if there are any clips to play.
        if (s.clips == null || s.clips.Length == 0)
        {
            Debug.LogWarning("SFX: " + name + " has no audio clips assigned.");
            return;
        }

        // Select a random clip from the array.
        AudioClip clipToPlay = s.clips[UnityEngine.Random.Range(0, s.clips.Length)];

        // Play the chosen clip if it's not null.
        if (clipToPlay != null)
        {
            // Use the overload of PlayOneShot that takes a volume scale.
            // This multiplies the sfxSource's master volume by the sound's individual volume.
            sfxSource.PlayOneShot(clipToPlay, s.volume);
        }
    }
    // --- MODIFIED END ---

    public void SetMusicVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);

        // Set volume for both sources, applying the rule for layer 2.
        if (musicSource1 != null) musicSource1.volume = volume;
        if (musicSource2 != null) musicSource2.volume = volume / 2f;

        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }

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
        SetMusicVolume(musicVolume);
        SetSfxVolume(sfxVolume);
    }
}


// --- MODIFIED START: Sound class now has an individual 'volume' slider ---
// A helper class to organize one or more AudioClips.
[System.Serializable]
public class Sound
{
    public string name;

    [Tooltip("The audio clip(s) for this sound. For SFX, a random clip is chosen. For music, the first clip is used.")]
    public AudioClip[] clips;

    [Tooltip("The chance (0.0 to 1.0) that this SFX will play when triggered. This does not affect music.")]
    [Range(0f, 1f)]
    public float chance = 1f;

    // --- NEW ---
    [Tooltip("Individual volume multiplier for this sound (0.0 to 1.0). This does not affect music.")]
    [Range(0f, 1f)]
    public float volume = 1f;
    // --- END NEW ---
}
// --- MODIFIED END ---


// A helper class to link a scene name to a music track name and layer.
[System.Serializable]
public class SceneMusic
{
    public string sceneName;
    public string musicName;
    [Tooltip("Which music layer to play this on (1 or 2).")]
    [Range(1, 2)]
    public int layer = 1;
}