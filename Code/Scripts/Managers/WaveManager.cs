using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WaveManager : MonoBehaviour {

    [Header("References")]
    [SerializeField] private Animator newWaveAnimator;

    [Header("Events")]
    public static UnityEvent onEnemyDestroy = new UnityEvent(); // Used to notify wave managers that enemies where killed

    // Special cases for tutorial
    public bool isTutorialOn = false;

    // We use both an index and class to tack day and wave which contain enemy info - UpdateCurrentDayAndWave function to keep in sync
    public int currentDayIndex = 0;
    public Day currentDay;
    public int currentWaveIndex = 0;
    public Wave currentWave;

    // Variables used to trigger new wave
    public int enemiesAlive = 0;
    public float timeBetweenWaves = 3;
    private float timeSinceLastWave = 0;

    // We use a spwan state to store and spawn enemies
    private List<SpawnState> spawnStates = new List<SpawnState>();
    private class SpawnState
    {
        public EnemyWaveInfo enemyInfo;
        public float timeSinceLastSpawn = 0f;
        public int spawnedCount = 0;
        public bool delayPassed = false;

        public SpawnState(EnemyWaveInfo info)
        {
            enemyInfo = info;
            timeSinceLastSpawn = -info.spawnDelay; // Start counting from the negative spawn delay
        }
    }

    private void Awake()
    {
        onEnemyDestroy.AddListener(EnemyDestroyed);
    }

    private void Start()
    {
        if (isTutorialOn)
        {
            Debug.Log("time between wave 10");
            timeBetweenWaves = 10; // To make it a bit more 
        }
        UpdateCurrentDayAndWave();
        if (currentDay != null && currentDay.waves.Count > 0 )
        {
            StartNextWave(); 
        }
    }

    // Function to take sync Day and wave index and class and trigger banner animation
    private void UpdateCurrentDayAndWave()
    {
        if (LevelManager.main.currentScenario.days.Count > currentDayIndex)
        {
            currentDay = LevelManager.main.currentScenario.days[currentDayIndex];
            if (currentDay.waves.Count > currentWaveIndex)
            {
                currentWave = currentDay.waves[currentWaveIndex];
                //if (!isTutorialOn){
                    TriggerWaveBannerAnimation();
                //}
            }
        }
    }

    private void TriggerWaveBannerAnimation() {
        // Text updating is done is the UI Menu script
        newWaveAnimator.SetTrigger("NewWaveAnimDisplay");
    }
    

    private void Update()
    {
        if ( currentDay == null || currentWaveIndex >= currentDay.waves.Count) return;

        int currentGameSpeed = LevelManager.GetGameSpeed();

        foreach (var state in spawnStates)
        {
            state.timeSinceLastSpawn += Time.deltaTime * currentGameSpeed;

            if (!state.delayPassed)
            {
                if (state.timeSinceLastSpawn >= 0) // Check if the delay has passed
                {
                    state.delayPassed = true;
                    state.timeSinceLastSpawn = 0; // Reset the timer to start interval spawning
                }
            }
            else if (state.spawnedCount < state.enemyInfo.quantity && state.timeSinceLastSpawn >= state.enemyInfo.spawnInterval)
            {
                SpawnEnemy(state.enemyInfo.enemyPrefab);
                state.spawnedCount++;
                state.timeSinceLastSpawn = 0; // Reset time since last spawn
            }
        }

        // If all enemies have been spawned for the current wave,
        // we start counting down to next wave spawn (ie not waiting until all enemy killed)
        if (spawnStates.TrueForAll(s => s.spawnedCount >= s.enemyInfo.quantity))
        {
            timeSinceLastWave += Time.deltaTime * currentGameSpeed;

             // If timer up and another wave in the same day --> Next wave
            if (currentWaveIndex + 1 < currentDay.waves.Count && timeSinceLastWave > timeBetweenWaves)
            {
                currentWaveIndex++;
                StartNextWave();
                timeSinceLastWave = 0; //Reset the counter
            }

            // If all ennemy dead and last wave --> Next Day OR Menu
            if (enemiesAlive == 0 && currentWaveIndex + 1 == currentDay.waves.Count)
            {;
                // We check if this is the last day
                if (LevelManager.main.currentScenario.days.Count == currentDayIndex + 1){
                    LevelManager.main.ExitToMainMenu(true);  // ScenarioComplete flag equal to true
                }
                else {
                    currentDayIndex = currentDayIndex+1;
                    currentWaveIndex = 0; // Reset so we start day + 1, wave 0 (of Day+1) 
                    StartNextWave();
                }
            }
        }
    }

    private void StartNextWave()
    {
        spawnStates.Clear();
        UpdateCurrentDayAndWave(); //sync indexes and classes and trigger banner animation

        foreach (EnemyWaveInfo enemyInfo in currentWave.enemies)
        {
            spawnStates.Add(new SpawnState(enemyInfo));
        }
    }

    private void SpawnEnemy(GameObject enemyPrefab)
    {
        // Randomize y a bit
        Vector3 originalPosition = LevelManager.main.startPoint.position;
        float randomOffsetY = Random.Range(-0.2f, 0.3f);
        // Apply the random offset to the original position
        Vector3 randomizedPosition = new Vector3(
            originalPosition.x,
            originalPosition.y + randomOffsetY,
            originalPosition.z // + randomOffsetZ for 3D games
        );
        
        Instantiate(enemyPrefab, randomizedPosition, Quaternion.identity);

        enemiesAlive++;
    }

    private void EnemyDestroyed()
    {
        LevelManager.main.DecreaseLives(1);
        enemiesAlive--;
    }
}