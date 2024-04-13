using UnityEngine;

//To-DO travel time scaling not working

public class WeatherManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject Sun;

    [Header("Sun Movement Properties")]
    public float travelTime = 800.0f; // Time it takes for the sun to travel across the screen under normal game speed

    private float elapsedTime = 0f;
    private Vector3 startPosition;
    private Vector3 endPosition;

    private void Start()
    {
        startPosition = Sun.transform.position; // Starting at the current position
        endPosition = new Vector3(startPosition.x + 24.66f, startPosition.y, startPosition.z); // Set the end position based on width
    }

    private void Update()
    {
        float currentGameSpeed = (float)LevelManager.GetGameSpeed();
        elapsedTime += Time.deltaTime * currentGameSpeed;

        // Calculate normalized time 't'
        float t = elapsedTime / travelTime;
        if (t > 1) {t = 1;} // Clamp t to prevent it from exceeding the end position}

        // Calculate horizontal position using linear interpolation
        float xPosition = Mathf.Lerp(startPosition.x, endPosition.x, t);

        // Calculate vertical position using a sine wave to create the arc
        float yPosition = Mathf.Sin(t * Mathf.PI) * 1; // Multiply by 0.95 to achieve the desired peak height of 1.9 units

        // Update the Sun's position
        Sun.transform.position = new Vector3(xPosition, startPosition.y + yPosition, startPosition.z);

        // Optionally, reset or handle the completion of the movement
        if (t >= 1) { }
    }

    private void ResetSunPosition() {
        elapsedTime = 0; // Reset time for continuous looping, if desired
        Sun.transform.position = startPosition; // Optionally reset to start position
    }
}
