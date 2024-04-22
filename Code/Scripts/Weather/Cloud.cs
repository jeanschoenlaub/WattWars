using UnityEngine;

public class Cloud: MonoBehaviour
{
    public Vector2 speed = new Vector2(-0.6f, -0.4f); // Speed in X and Y directions
    public Vector2 cloudSize; // Stores the scale/size of the cloud for shade calculation

    public void SetSize(float size)
    {
        transform.localScale = new Vector3(size, size, size);
        cloudSize = new Vector2 (0.16f*size, 0.12f*size) ; //0.12 is the initiat cload size
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
            WeatherManager.main.RemoveCloudFromActiveClouds(this);
        }
    }

    // Method to determine if a given position is in the shade of this cloud
    public bool IsInShade(Vector3 worldPosition)
    {

        float cloudMaxX = transform.position.x + cloudSize.x;
        float cloudMinX = transform.position.x - cloudSize.x;
        float cloudMaxY = transform.position.y + cloudSize.y;
        float cloudMinY = transform.position.y - cloudSize.y;

        // Debug.Log("TowerPos: ");
        // Debug.Log("TowerPos: " + worldPosition.x.ToString() + " , " +  worldPosition.y.ToString() );
        // Debug.Log("CloudPos: " + transform.position.x.ToString() + " , " +  transform.position.y.ToString() );
        // Debug.Log("CloudMaxX: " + cloudMaxX.ToString());
        // Debug.Log("CloudMinX: " + cloudMinX.ToString());
        // Debug.Log("CloudMaxY: " + cloudMaxY.ToString());
        // Debug.Log("CloudMinY: " + cloudMinY.ToString());

        if (worldPosition.x >= cloudMinX && worldPosition.x <= cloudMaxX &&
            worldPosition.y >= cloudMinY && worldPosition.y <= cloudMaxY)
        {
            return true; // The worldPosition is within the cloud bounds
        }
        else
        {
            //Debug.Log("The position is outside the cloud.");
            return false; // The worldPosition is outside the cloud bounds
        }

    }
}
