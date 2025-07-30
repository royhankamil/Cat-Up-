using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [Header("Parallax Layers")]
    [Tooltip("The layers to be affected by the parallax effect. The layer at index 0 is the furthest back, and the last layer is the furthest forward.")]
    public GameObject[] layers;

    [Header("Parallax Intensity")]
    [Tooltip("Controls the overall strength of the parallax effect. X for horizontal, Y for vertical.")]
    public Vector2 parallaxIntensity = new Vector2(1f, 1f);

    // The camera that the parallax effect will follow
    public Transform cameraTransform;
    // The starting position of the camera when the scene loads
    private Vector3 cameraStartPosition;
    // The starting positions of each parallax layer
    private Vector3[] layerStartPositions;

    void Start()
    {
        // Store the camera's starting position
        cameraStartPosition = cameraTransform.position;

        // Initialize and store the starting positions of each layer
        layerStartPositions = new Vector3[layers.Length];
        for (int i = 0; i < layers.Length; i++)
        {
            if (layers[i] != null)
            {
                layerStartPositions[i] = layers[i].transform.position;
            }
        }
    }

    void LateUpdate()
    {
        // Calculate how much the camera has moved from its starting position
        Vector2 cameraDelta = cameraTransform.position - cameraStartPosition;

        // Loop through each layer to apply the parallax effect
        for (int i = 0; i < layers.Length; i++)
        {
            if (layers[i] == null) continue;

            // The parallax strength is determined by the layer's order.
            // Layers further back (lower index) move less than layers closer to the front.
            // The strength is a value between 0 (moves with camera) and 1 (doesn't move).
            float parallaxStrength = (float)i / (layers.Length - 1);
            
            // If there's only one layer, it should be the furthest back (no parallax)
            if (layers.Length <= 1)
            {
                parallaxStrength = 1f;
            }

            // Calculate the distance the layer should move based on camera delta, strength, and overall intensity
            float moveX = cameraDelta.x * parallaxStrength * parallaxIntensity.x;
            float moveY = cameraDelta.y * parallaxStrength * parallaxIntensity.y;

            // Calculate the new position for the layer
            Vector3 newPosition = new Vector3(layerStartPositions[i].x + moveX, layerStartPositions[i].y + moveY, layerStartPositions[i].z);

            // Apply the new position to the layer's transform
            layers[i].transform.position = newPosition;
        }
    }
}
