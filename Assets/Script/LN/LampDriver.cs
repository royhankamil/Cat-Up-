using UnityEngine;
using DG.Tweening; // Make sure you have DOTween imported into your project
using UnityEngine.Rendering.Universal; // Required for Light2D

/// <summary>
/// Animates an array of Light2D components to create a natural, flickering effect.
/// Uses DOTween for smooth, performant, and randomized animations.
/// </summary>
public class LampDriver : MonoBehaviour
{
    [Header("Setup")]
    [Tooltip("The array of Light2D objects to be animated.")]
    public Light2D[] lamps;

    [Header("Flicker Intensity")]
    [Tooltip("The minimum intensity the light will flicker to.")]
    public float minIntensity = 0.8f;
    [Tooltip("The maximum intensity the light will flicker to.")]
    public float maxIntensity = 1.2f;

    [Header("Flicker Radius (Breath)")]
    [Tooltip("The minimum outer radius the light will flicker to, as a multiplier of its original radius.")]
    public float minOuterRadiusMultiplier = 0.95f;
    [Tooltip("The maximum outer radius the light will flicker to, as a multiplier of its original radius.")]
    public float maxOuterRadiusMultiplier = 1.05f;

    [Header("Flicker Timing")]
    [Tooltip("The minimum time for one flicker transition.")]
    public float minFlickerDuration = 0.05f;
    [Tooltip("The maximum time for one flicker transition.")]
    public float maxFlickerDuration = 0.2f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Check if any lamps have been assigned
        if (lamps == null || lamps.Length == 0)
        {
            Debug.LogWarning("LampDriver: No Light2D objects have been assigned in the Inspector.");
            return;
        }

        // Initialize DOTween if it hasn't been already.
        DOTween.Init();

        // Start the flicker animation for each assigned lamp
        foreach (Light2D lamp in lamps)
        {
            if (lamp != null)
            {
                // Store original values to be used as a base for radius animation
                lamp.gameObject.AddComponent<OriginalLightValues>().Store(lamp.pointLightOuterRadius);
                Flicker(lamp);
            }
        }
    }

    /// <summary>
    /// Starts a recursive flicker animation loop for a single Light2D.
    /// </summary>
    /// <param name="lamp">The Light2D component to animate.</param>
    private void Flicker(Light2D lamp)
    {
        if (lamp == null) return; // Stop if the lamp has been destroyed

        // Get the original radius stored on the lamp's GameObject
        float originalRadius = lamp.GetComponent<OriginalLightValues>().OriginalRadius;

        // Create a sequence for combined animations
        Sequence sequence = DOTween.Sequence();

        // Add the intensity tween to the sequence using a generic tween
        sequence.Append(
            DOTween.To(() => lamp.intensity, x => lamp.intensity = x, Random.Range(minIntensity, maxIntensity), Random.Range(minFlickerDuration, maxFlickerDuration))
        );

        // Add the radius tween to run at the same time as the intensity tween
        sequence.Insert(0,
            DOTween.To(() => lamp.pointLightOuterRadius, x => lamp.pointLightOuterRadius = x, originalRadius * Random.Range(minOuterRadiusMultiplier, maxOuterRadiusMultiplier), Random.Range(minFlickerDuration, maxFlickerDuration))
        );
        
        // When the sequence is complete, call this function again to loop indefinitely
        sequence.OnComplete(() => Flicker(lamp));
    }

    /// <summary>
    /// OnDestroy is called when the MonoBehaviour will be destroyed.
    /// It's good practice to kill all tweens associated with this object to prevent memory leaks.
    /// </summary>
    void OnDestroy()
    {
        if (lamps == null) return;

        // Kill all tweens associated with the lamps to clean up
        foreach (Light2D lamp in lamps)
        {
            if (lamp != null)
            {
                // DOTween.Kill(target) kills all tweens running on the specified target.
                DOTween.Kill(lamp);
            }
        }
    }
}

/// <summary>
/// A helper component to store the original radius of a Light2D.
/// This is added at runtime and should not be added manually.
/// </summary>
internal class OriginalLightValues : MonoBehaviour
{
    internal float OriginalRadius { get; private set; }
    internal void Store(float radius)
    {
        OriginalRadius = radius;
    }
}
