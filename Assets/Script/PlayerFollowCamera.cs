using UnityEngine;

public class FollowPlayerYAxis : MonoBehaviour
{
    [Tooltip("The player object the camera should track.")]
    public Transform playerTransform;

    [Tooltip("A vertical offset from the player's position.")]
    public float yOffset = 0f;

    // A reference to this object's transform.
    private Transform cameraFollowTarget;

    void Start()
    {
        // Get the transform of this GameObject (the CameraFollowTarget).
        cameraFollowTarget = transform;

        if (playerTransform == null)
        {
            Debug.LogError("Player Transform is not assigned!");
            enabled = false;
        }
    }

    // Use LateUpdate for camera movements to prevent jitter.
    void LateUpdate()
    {
        if (playerTransform == null) return;
        
        // Directly set the target's Y position to match the player's Y position.
        // The X and Z positions remain unchanged.
        cameraFollowTarget.position = new Vector3(
            cameraFollowTarget.position.x,
            playerTransform.position.y + yOffset,
            cameraFollowTarget.position.z
        );
    }
}