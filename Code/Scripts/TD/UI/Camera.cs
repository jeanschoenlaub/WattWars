using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraAdjuster : MonoBehaviour
{
    public float targetAspect = 16f / 10f; // Target aspect ratio you designed for
    public float targetOrthographicSize = 6.4f; // Target orthographic size

    void Start()
    {
        AdjustCameraBasedOnSafeArea();
    }

    void AdjustCameraBasedOnSafeArea()
    {
        Camera camera = GetComponent<Camera>();

        // Calculate the safe area
        Rect safeArea = Screen.safeArea;
        Vector2 safeAreaSize = new Vector2(safeArea.width, safeArea.height);
        float safeAspect = safeAreaSize.x / safeAreaSize.y;

        // Adjust camera size based on the safe area
        if (safeAspect < targetAspect)
        {
            // If the screen is narrower than the target aspect, adjust orthographic size to ensure content fits
            camera.orthographicSize = targetOrthographicSize * (targetAspect / safeAspect);
        }
        else
        {
            // If the screen is wider, we might need to adjust differently or not at all, depending on your game's design
            // For simplicity, this example does not scale the camera up, assuming that the essential gameplay area remains visible
            // You could implement additional logic here if your game needs to handle wider screens differently
            camera.orthographicSize = targetOrthographicSize;
        }
    }
}
