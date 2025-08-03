using UnityEngine;
using DG.Tweening;
public class ClickedEffector : MonoBehaviour
{
    public void AnimateButtonClick(Transform buttonTransform)
    {
        // Kill any existing animations on this transform before starting a new one.
        // The 'true' parameter forces the animation to its completed state (original scale).
        buttonTransform.DOKill(true);

        // Example of playing a sound effect. Uncomment if you have an AudioManager.
        if (AudioManager.Instance != null)
            // Ensure you have an AudioManager script with a PlaySfx method.
            AudioManager.Instance.PlaySfx("Button Click");

        buttonTransform.DOPunchScale(
            punch: new Vector3(0.15f, 0.15f, 0.15f),
            duration: 0.3f,
            vibrato: 5,
            elasticity: 1);
    }
}
