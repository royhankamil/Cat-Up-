// Copyright (c) 2025 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using UnityEngine;
using TMPro; // Required for TextMeshPro elements
using DG.Tweening; // Required for DOTween animations

/// <summary>
/// Animates a UI logo with a floating effect and a TextMeshProUGUI element
/// with a fading "press to start" effect using DOTween.
/// </summary>
public class MainMenuAnimator : MonoBehaviour
{
    [Header("UI Element Assignments")]
    [Tooltip("Assign the RectTransform of the logo you want to animate.")]
    public RectTransform logoTransform, logoTransform2;

    [Tooltip("Assign the TextMeshProUGUI element for the 'Press to Start' text.")]
    public TextMeshProUGUI startText;

    [Header("Logo Animation Settings")]
    [Tooltip("The vertical distance the logo will float up and down.")]
    public float floatHeight = 30f;

    [Tooltip("Controls the speed of the logo's floating animation. Higher value = faster.")]
    [Range(0.1f, 5f)]
    public float logoFloatSpeed = 1f;

    [Header("Text Animation Settings")]
    [Tooltip("Controls the speed of the text's fade animation. Higher value = faster.")]
    [Range(0.1f, 5f)]
    public float textFadeSpeed = 0.8f;

    // Store the initial position of the logo to calculate the float path.
    private Vector3 initialLogoPosition;

    void Start()
    {
        // --- Safety Checks ---
        // Ensure the logo transform is assigned before trying to animate it.
        if (logoTransform == null)
        {
            Debug.LogError("Logo Transform is not assigned in the inspector! Cannot start animation.");
            return; // Stop the script if the logo is not set
        }

        // Ensure the start text is assigned before trying to animate it.
        if (startText == null)
        {
            Debug.LogError("Start Text is not assigned in the inspector! Cannot start animation.");
            return; // Stop the script if the text is not set
        }

        // Store the starting anchored position of the logo.
        initialLogoPosition = logoTransform.anchoredPosition;

        // Start the animations.
        AnimateLogo();
        AnimateStartText();
    }

    /// <summary>
    /// Creates a smooth, looping floating animation for the logo.
    /// </summary>
    private void AnimateLogo()
    {
        // Calculate the duration based on the desired speed.
        // A higher speed results in a shorter duration for the tween.
        float duration = 1f / logoFloatSpeed;

        // Animate the logo's Y position.
        logoTransform.DOAnchorPosY(initialLogoPosition.y + floatHeight, duration)
            .SetEase(Ease.InOutSine) // Use a smooth sine wave easing for a natural float.
            .SetLoops(-1, LoopType.Yoyo); // Loop indefinitely, moving back and forth (Yoyo).
    }

    public void animateStartMenuLogo()
    {
        logoTransform2.DOAnchorPosY(logoTransform2.transform.localPosition.y + 30, 2f)
        .SetEase(Ease.InOutSine) // Use a smooth sine wave easing for a natural float.
        .SetLoops(-1, LoopType.Yoyo); // Loop indefinitely, moving back and forth (Yoyo).
    }

    /// <summary>
    /// Creates a smooth, looping fade animation for the text.
    /// </summary>
    private void AnimateStartText()
    {
        // Calculate the duration based on the desired speed.
        float duration = 1f / textFadeSpeed;

        // Animate the text's alpha (transparency) from its current value to 0.
        startText.DOFade(0f, duration)
            .SetEase(Ease.InOutQuad) // A simple ease-in and ease-out effect.
            .SetLoops(-1, LoopType.Yoyo); // Loop indefinitely, fading in and out.
    }

    /// <summary>
    /// OnDestroy is called when the object is destroyed.
    /// It's good practice to kill all tweens associated with this object
    /// to prevent potential memory leaks or errors if the scene changes.
    /// </summary>
    void OnDestroy()
    {
        // Kill tweens associated with the specific elements to be safe.
        if (logoTransform != null)
        {
            logoTransform.DOKill();
        }
        if (startText != null)
        {
            startText.DOKill();
        }
    }
}
