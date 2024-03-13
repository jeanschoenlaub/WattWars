using System.Collections;
using UnityEngine;

public class MysteryBoxManager : MonoBehaviour
{
    public GameObject mysteryBoxPrefab;

    // Spawn and position variables
    private Vector2 spawnPoint = new Vector2(-11f, 0f); 
    public float minY = -5f; // Minimum Y offset from the spawn point
    public float maxY = 5f; // Maximum Y offset from the spawn point
    public float minSpawnDelay = 20f; // Minimum delay before a new box spawns
    public float maxSpawnDelay = 50f; // Maximum delay before a new box spawns

    private void Start()
    {
        StartCoroutine(SpawnMysteryBoxAtRandomIntervals());
    }

     private IEnumerator SpawnMysteryBoxAtRandomIntervals()
    {
        while (true)
        {
            // If there's no Mystery Box present, start the spawn process
            if (FindObjectOfType<MysteryBox>() == null)
            {
                float elapsed = 0f; // Elapsed time since we started waiting
                float waitTime = Random.Range(minSpawnDelay, maxSpawnDelay); // Total time we want to wait
                // Wait until enough time has elapsed
                while (elapsed < waitTime)
                {
                    yield return null; // Wait for the next frame
                    elapsed += Time.deltaTime * LevelManager.GetGameSpeed();
                }

                // Spawn the Mystery Box
                SpawnMysteryBox();
            }
            else
            {
                // If a Mystery Box is present, wait a bit before checking again
                yield return new WaitForSeconds(1f);
            }
        }

    }

    private void SpawnMysteryBox()
    {
        float randomY = Random.Range(minY, maxY);
        Vector3 spawnPosition = new Vector3(spawnPoint.x, spawnPoint.y + randomY, 0);
        Instantiate(mysteryBoxPrefab, spawnPosition, Quaternion.identity);
    }
}