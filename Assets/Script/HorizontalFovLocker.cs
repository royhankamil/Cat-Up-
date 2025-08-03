using UnityEngine;
using Unity.Cinemachine;

// This script should be attached to the same GameObject as an ORTHOGRAPHIC CinemachineVirtualCamera.
[RequireComponent(typeof(CinemachineCamera))]
public class HorizontalOrthoSizeLocker : MonoBehaviour
{
    private CinemachineCamera vcam;

    [Tooltip("The desired constant horizontal width for the camera's view.")]
    [SerializeField] private float horizontalWidth = 20f;

    // Cache the vcam component
    void Awake()
    {
        vcam = GetComponent<CinemachineCamera>();
        if (!vcam.Lens.Orthographic)
        {
            Debug.LogWarning("HorizontalOrthoSizeLocker requires the CinemachineCamera to be in Orthographic mode.", this);
        }
    }

    // We use LateUpdate to ensure the size is set after all other camera updates.
    void LateUpdate()
    {
        // Ensure the camera is orthographic before proceeding
        if (!vcam.Lens.Orthographic) return;

        // Get the current aspect ratio of the screen.
        float currentAspect = (float)Screen.width / Screen.height;

        // Avoid division by zero
        if (currentAspect <= 0) return;

        // Calculate the new orthographic size.
        // Unity's OrthographicSize is half of the vertical screen height.
        // We calculate the required vertical size to maintain a constant horizontal width.
        float newOrthoSize = horizontalWidth / (2f * currentAspect);

        vcam.Lens.OrthographicSize = newOrthoSize;
    }
}