using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraAdjuster : MonoBehaviour
{
    public float targetAspect = 16f / 10f; // Target aspect ratio you designed for
    public float targetOrthographicSize = 6.4f; // Target orthographic size

    void Start()
    {
        Camera camera = GetComponent<Camera>();
        float currentAspect = (float)Screen.width / Screen.height;
        camera.orthographicSize = targetOrthographicSize * (targetAspect / currentAspect);
    }
}