using UnityEngine;
using Unity.Cinemachine;

// This script should be attached to the same GameObject as a CinemachineVirtualCamera.
[RequireComponent(typeof(CinemachineCamera))]
public class HorizontalOrthoSizeLocker : MonoBehaviour
{
    [Tooltip("The desired constant horizontal width for the camera's view.")]
    [SerializeField] private float horizontalWidth = 20f;

    private CinemachineCamera vcam;
    private CinemachineBrain brain;

    void Start()
    {
        vcam = GetComponent<CinemachineCamera>();

        // Find the brain using the modern, recommended method
        brain = FindFirstObjectByType<CinemachineBrain>();

        if (brain == null)
        {
            Debug.LogError("A CinemachineBrain is required in the scene but was not found.", this);
            enabled = false; // Disable the script if the brain is missing.
            return;
        }

    }

    private void LateUpdate()
    {
        // CHANGED: This is the corrected check for the active camera
        if (brain != null && brain.ActiveVirtualCamera == vcam)
        {
            UpdateOrthoSize();
        }
    }

    void UpdateOrthoSize()
    {
        // The brain reference is checked in LateUpdate, so we can safely use it here.
        Camera outputCamera = brain.OutputCamera;
        if (outputCamera == null) return;

        float screenHeight = outputCamera.pixelHeight;
        float screenWidth = outputCamera.pixelWidth;

        if (screenHeight <= 0) return;

        float currentAspect = screenWidth / screenHeight;
        if (currentAspect <= 0) return;

        vcam.Lens.OrthographicSize = horizontalWidth / (2f * currentAspect);
    }
}