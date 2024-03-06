
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WaveManager : MonoBehaviour {

    //For now manually setting wave
    [Header("Wave Configuration")]
    [SerializeField] private Day currentDay; // Assign this in the Inspector

    [Header("Events")]
    public static UnityEvent onEnemyDestroy = new UnityEvent();

    private int currentWaveIndex = 0;
    private bool isSpawning = false;
    public int enemiesAlive = 0;

    // Store the spawn state directly using your existing structure
    private List<SpawnState> spawnStates = new List<SpawnState>();

    private class SpawnState
    {
        public EnemyWaveInfo enemyInfo;
        public float timeSinceLastSpawn = 0f;
        public int spawnedCount = 0;

        public SpawnState(EnemyWaveInfo info)
        {
            enemyInfo = info;
        }
    }

    private void Awake()
    {
        onEnemyDestroy.AddListener(EnemyDestroyed);
    }

    private void Start()
    {
       if (currentDay != null && currentDay.waves.Count > 0)
        {
            StartNextWave(); // Start the first wave
        }
    }

    private void Update()
    {
        if (!isSpawning || currentDay == null || currentWaveIndex >= currentDay.waves.Count) return;

        int currentGameSpeed = LevelManager.GetGameSpeed();
        if (currentGameSpeed <= 0) return; // Pause if game speed is 0

        foreach (var state in spawnStates)
        {
            state.timeSinceLastSpawn += Time.deltaTime * currentGameSpeed;

            if (state.spawnedCount < state.enemyInfo.quantity && state.timeSinceLastSpawn >= state.enemyInfo.spawnInterval)
            {
                SpawnEnemy(state.enemyInfo.enemyPrefab);
                state.spawnedCount++;
                state.timeSinceLastSpawn = 0;

                // If all enemies of this type have been spawned, continue checking other types
            }
        }

        // Check if all enemies have been killed spawned for the current wave
        if (enemiesAlive == 0 && spawnStates.TrueForAll(s => s.spawnedCount >= s.enemyInfo.quantity))
        {
            isSpawning = false;
            // Prepare for the next wave or handle the end of the day
            if (currentWaveIndex + 1 < currentDay.waves.Count)
            {
                currentWaveIndex++;
                StartNextWave();
            }
            else
            {
                Debug.Log("All waves for the day completed.");
                // Handle day completion here
            }
        }
    }

    private void StartNextWave()
    {
        Wave currentWave = currentDay.waves[currentWaveIndex];
        spawnStates.Clear();

        foreach (EnemyWaveInfo enemyInfo in currentWave.enemies)
        {
            spawnStates.Add(new SpawnState(enemyInfo));
        }

        isSpawning = true;
    }

    private void SpawnEnemy(GameObject enemyPrefab)
    {
        Instantiate(enemyPrefab, LevelManager.main.startPoint.position, Quaternion.identity);
        enemiesAlive++;
    }

    private void EnemyDestroyed()
    {
        enemiesAlive--;
    }
}