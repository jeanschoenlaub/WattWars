using UnityEngine;
using System.Collections.Generic;


public class WeatherManager : MonoBehaviour
{
    public static WeatherManager main;

    [Header("References")]
    [SerializeField] private GameObject sunGO;
    [SerializeField] private GameObject skyCloudGO;
    public GameObject[] cloudPrefabs; // Array to hold different cloud prefabs.
    private List<Cloud> activeClouds = new List<Cloud>(); // List to store active cloud instances


    [Header("Weather Parameters")]
    public float sunTravelTime = 800.0f; // Time it takes for the sun to travel across the screen under normal game speed
    public float cloudSpawnRate = 25f; // Time between spawns.
    private bool cloudsonStart = true; // Bool to control if there should be clouds at the beginning of the game

    //Parameters
    private float nextSpawnTime = 0f;
    private float elapsedTime = 0f;
    private Vector3 startPositionSun;
    private Vector3 startPositionCloud;
    private Vector3 endPositionSun;

    private void Awake()
    {
        main = this;
    }   

    
    private void Start()
    {
        startPositionSun = sunGO.transform.position; // Starting at the current position
        startPositionCloud = skyCloudGO.transform.position; // Starting at the current position
        endPositionSun = new Vector3(startPositionSun.x + 24.66f, startPositionSun.y, startPositionSun.z); // Set the end position based on width
    
        if (cloudsonStart){
            Vector3 spawnPositionOnMap = new Vector3  (
                UnityEngine.Random.Range(0, 6), // x position outside of map to the right
                UnityEngine.Random.Range(0, 4), // Y range for clouds.
                -1// Zs To be above other Game objects
            );
            SpawnCloud(spawnPositionOnMap);
        }
    }

    private void Update()
    {
        float currentGameSpeed = (float)LevelManager.GetGameSpeed();
        elapsedTime += Time.deltaTime * currentGameSpeed;

        // Calculate normalized time 't'
        float t = elapsedTime / sunTravelTime;
        if (t > 1) {t = 1;} // Clamp t to prevent it from exceeding the end position}

        // Calculate horizontal position using linear interpolation and vertical position using a sine wave to create the arc
        float xPosition = Mathf.Lerp(startPositionSun.x, endPositionSun.x, t);
        float yPosition = Mathf.Sin(t * Mathf.PI) * 1; // Multiply by 0.95 to achieve the desired peak height of 1.9 units

        // Update the Sun's position
        sunGO.transform.position = new Vector3(xPosition, startPositionSun.y + yPosition, startPositionSun.z);
        skyCloudGO.transform.position = new Vector3(xPosition/2 - startPositionCloud.x, startPositionCloud.y, startPositionCloud.y);

        if (elapsedTime >= nextSpawnTime)
        {
            SpawnCloud();
            nextSpawnTime = elapsedTime + cloudSpawnRate;
        }
    }

    private void ResetSunPosition() {
        elapsedTime = 0; // Reset time for continuous looping, if desired
        sunGO.transform.position = startPositionSun; // Optionally reset to start position
    }

    void SpawnCloud(Vector3? spawnPosition = null)
    {
        GameObject cloudPrefab = cloudPrefabs[UnityEngine.Random.Range(0, cloudPrefabs.Length)];
        
        // Determine whether to randomize the X or Y position
        bool randomizeX = UnityEngine.Random.value > 0.5f; // Randomly choose true or false

        Vector3 position;
        if (spawnPosition == null)
        {
            if (randomizeX) // Random X position, fixed Y position at the top of screen
            {
                position = new Vector3(UnityEngine.Random.Range(-5, 10), 8, -1); // Fixed - 1 Z position above other objects
            }
            else  // Random Y position, fixed X position at the right of screen
            {
              position = new Vector3(16, UnityEngine.Random.Range(0,15), -1); // Fixed - 1 Z position above other objects
            }
        }
        else
        {
            position = (Vector3)spawnPosition;
        }
       
        //Instantiate and access main cloud script 
        GameObject cloudGameObject = Instantiate(cloudPrefab, position, Quaternion.identity);
        Cloud cloudComponent = cloudGameObject.GetComponent<Cloud>();
        
        // Randomize size
        float randomScale = UnityEngine.Random.Range(20, 40); // Adjust scale range as needed

        if (cloudComponent != null)
        {
            activeClouds.Add(cloudComponent);
            cloudComponent.SetSize(randomScale);
        }
    }

    // Public method to check if a position is in the shade of any active cloud
    public bool CheckIfInTheShadeOfAnyActiveCloud(Vector3 worldPosition)
    {
        foreach (Cloud cloud in activeClouds)
        {
            if (cloud.IsInShade(worldPosition))
            {
                Debug.Log("One Cloud hit");
                return true; // Return true if the position is within the bounds of any cloud
            }
        }
        return false; // Return false if no clouds shade the position
    }

    public void RemoveCloudFromActiveClouds(Cloud cloud)
    {
        if (activeClouds.Contains(cloud))
        {
            activeClouds.Remove(cloud);
        }
    }
}
