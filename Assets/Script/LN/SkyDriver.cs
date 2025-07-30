using UnityEngine;
using DG.Tweening; // Make sure you have DOTween imported into your project

/// <summary>
/// Animates the child objects of specified transforms to create glimmering stars and drifting clouds.
/// This script uses DOTween for smooth and performant animations with added randomness for a more natural feel.
/// </summary>
public class SkyDriver : MonoBehaviour
{
    [Header("Star Setup")]
    [Tooltip("The parent GameObject containing all the star objects to be animated.")]
    public Transform starsContainer;

    [Header("Cloud Setup")]
    [Tooltip("The parent GameObject containing all the cloud objects to be animated.")]
    public Transform cloudsContainer;

    [Header("Star Glimmer & Scale")]
    [Tooltip("The minimum scale a star will reach during its glimmer animation.")]
    [Range(1f, 2f)]
    public float glimmerMaxScaleMin = 1.1f;

    [Tooltip("The maximum scale a star will reach during its glimmer animation.")]
    [Range(1f, 3f)]
    public float glimmerMaxScaleMax = 1.5f;

    [Tooltip("The minimum duration for a single glimmer (scale up and down) cycle.")]
    public float glimmerDurationMin = 1.0f;

    [Tooltip("The maximum duration for a single glimmer (scale up and down) cycle.")]
    public float glimmerDurationMax = 3.0f;

    [Header("Star Fade")]
    [Tooltip("The minimum alpha (transparency) a star will fade to. 0 is fully transparent, 1 is fully opaque.")]
    [Range(0f, 1f)]
    public float glimmerMinAlpha = 0.4f;

    [Tooltip("The maximum alpha (transparency) a star will fade to.")]
    [Range(0f, 1f)]
    public float glimmerMaxAlpha = 1.0f;

    [Header("Star Movement")]
    [Tooltip("The minimum distance a star can move from its original position.")]
    public float moveIntensityMin = 0.2f;

    [Tooltip("The maximum distance a star can move from its original position.")]
    public float moveIntensityMax = 0.8f;

    [Tooltip("The minimum duration for a single star movement cycle.")]
    public float moveDurationMin = 5.0f;

    [Tooltip("The maximum duration for a single star movement cycle.")]
    public float moveDurationMax = 10.0f;

    [Header("Cloud Movement")]
    [Tooltip("The horizontal distance the clouds will drift from their starting point.")]
    public float cloudMoveDistance = 2.0f;

    [Tooltip("The minimum duration for a single cloud drift cycle (back and forth).")]
    public float cloudMoveDurationMin = 20.0f;

    [Tooltip("The maximum duration for a single cloud drift cycle (back and forth).")]
    public float cloudMoveDurationMax = 40.0f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialize DOTween if it hasn't been already.
        DOTween.Init();

        // Animate Stars if the container is assigned
        if (starsContainer != null)
        {
            foreach (Transform star in starsContainer)
            {
                AnimateStar(star);
            }
        }

        // Animate Clouds if the container is assigned
        if (cloudsContainer != null)
        {
            foreach (Transform cloud in cloudsContainer)
            {
                AnimateCloud(cloud);
            }
        }
    }

    /// <summary>
    /// Applies looping glimmer, fade, and movement animations to a single star transform.
    /// </summary>
    private void AnimateStar(Transform star)
    {
        Vector3 originalScale = star.localScale;
        Vector3 originalPosition = star.localPosition;
        SpriteRenderer starSprite = star.GetComponent<SpriteRenderer>();

        // --- Glimmer (Scale & Fade) Animation ---
        float glimmerDuration = Random.Range(glimmerDurationMin, glimmerDurationMax);
        float glimmerScale = Random.Range(glimmerMaxScaleMin, glimmerMaxScaleMax);
        
        star.DOScale(originalScale * glimmerScale, glimmerDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo)
            .SetDelay(Random.Range(0f, glimmerDuration));

        if (starSprite != null)
        {
            float glimmerAlpha = Random.Range(glimmerMinAlpha, glimmerMaxAlpha);
            starSprite.DOFade(glimmerAlpha, glimmerDuration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo)
                .SetDelay(Random.Range(0f, glimmerDuration));
        }

        // --- Movement Animation ---
        float moveDuration = Random.Range(moveDurationMin, moveDurationMax);
        float moveIntensity = Random.Range(moveIntensityMin, moveIntensityMax);

        star.DOLocalMove(originalPosition + (Vector3)Random.insideUnitCircle * moveIntensity, moveDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo)
            .SetDelay(Random.Range(0f, moveDuration));
    }

    /// <summary>
    /// Applies a slow, looping horizontal drift to a single cloud transform.
    /// </summary>
    private void AnimateCloud(Transform cloud)
    {
        float originalX = cloud.localPosition.x;
        float moveDuration = Random.Range(cloudMoveDurationMin, cloudMoveDurationMax);

        cloud.DOLocalMoveX(originalX + cloudMoveDistance, moveDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo)
            .SetDelay(Random.Range(0f, moveDuration)); // Stagger the cloud movements
    }

    /// <summary>
    /// OnDestroy is called when the MonoBehaviour will be destroyed.
    /// It's good practice to kill all tweens associated with this object to prevent memory leaks.
    /// </summary>
    void OnDestroy()
    {
        // Kill all tweens associated with the stars
        if (starsContainer != null)
        {
            foreach (Transform star in starsContainer)
            {
                if(star != null)
                {
                    DOTween.Kill(star);
                    SpriteRenderer starSprite = star.GetComponent<SpriteRenderer>();
                    if (starSprite != null)
                    {
                        DOTween.Kill(starSprite);
                    }
                }
            }
        }

        // Kill all tweens associated with the clouds
        if (cloudsContainer != null)
        {
            foreach (Transform cloud in cloudsContainer)
            {
                if (cloud != null)
                {
                    DOTween.Kill(cloud);
                }
            }
        }
    }
}
