using UnityEngine;

public class CloudMovement : MonoBehaviour
{
    public Vector2 speed = new Vector2(-0.6f, -0.4f); // Speed in X and Y directions
    private float cloudSize; // Stores the scale/size of the cloud for shade calculation

    public void SetSize(float size)
    {
        cloudSize = size;
    }

    void Update()
    {
        // Apply movement to the cloud's position
        int currentGameSpeed = LevelManager.GetGameSpeed();
        Vector3 movement = new Vector3(
            speed.x * Time.deltaTime* currentGameSpeed,
             speed.y * Time.deltaTime* currentGameSpeed,
              0)
            ;
        transform.Translate(movement);

        if (transform.position.x < -20) // Define boundary as needed
        {
            Destroy(gameObject);
            Debug.Log("destroying cloud");
        }
    }

    // Method to determine if a given position is in the shade of this cloud
    public bool IsInShade(Vector3 worldPosition)
    {
        float cloudRadius = cloudSize * 0.5f; // Assuming the sprite is square and centered
        Bounds cloudBounds = new Bounds(transform.position, new Vector3(cloudRadius, cloudRadius, 0));

        return cloudBounds.Contains(worldPosition);
    }
}
