
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WaveManager : MonoBehaviour {

    //For now manually setting wave
    [Header("References")]
    [SerializeField] private Day currentDay; // Assign this in the Inspector
    [SerializeField] private Animator newWaveAnimator;

    [Header("Events")]
    public static UnityEvent onEnemyDestroy = new UnityEvent();

    public int currentWaveIndex = 0;
    public int enemiesAlive = 0;

    public float timeBetweenWaves = 3;
    private float timeSinceLastWave = 0;

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
            //  Display
            newWaveAnimator.SetBool("NewWaveAnimDisplay", true);
            StartNextWave(); 
        }
    }

    private void Update()
    {
        if ( currentDay == null || currentWaveIndex >= currentDay.waves.Count) return;

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

        // If all enemies have been spawned for the current wave, we apply a wave delta and spawn the next wave
        if (spawnStates.TrueForAll(s => s.spawnedCount >= s.enemyInfo.quantity))
        {
            //  Wave spawining just finished
            if (timeSinceLastWave == 0 && newWaveAnimator) {
                newWaveAnimator.SetBool("NewWaveAnimDisplay", true);
            }
            timeSinceLastWave += Time.deltaTime * currentGameSpeed;
            // If there is another wave left we have a little timer and animation
            if (currentWaveIndex + 1 < currentDay.waves.Count && timeSinceLastWave > timeBetweenWaves )
            {
                Debug.Log("Spawning next wave");
                currentWaveIndex++;
                StartNextWave();
                // We reset animation and counter
                newWaveAnimator.SetBool("NewWaveAnimDisplay", false);
                timeSinceLastWave = 0; //Reset the counter
            }
            // If all ennemy dead and last wave --> end Day
            if (enemiesAlive == 0 && currentWaveIndex + 1 == currentDay.waves.Count)
            {
                Debug.Log("All waves for the day completed.");
                newWaveAnimator.SetBool("NewWaveAnimDisplay", false);
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