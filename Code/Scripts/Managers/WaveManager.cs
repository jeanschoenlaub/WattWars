using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

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
        if (isTutorialOn)
        {
            timeBetweenWaves = 0; // To start sooner
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
                if (!isTutorialOn){TriggerWaveBannerAnimation();}
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

            if (state.spawnedCount < state.enemyInfo.quantity && state.timeSinceLastSpawn >= state.enemyInfo.spawnInterval)
            {
                SpawnEnemy(state.enemyInfo.enemyPrefab);
                state.spawnedCount++;
                state.timeSinceLastSpawn = 0;
                // If all enemies of this type have been spawned, continue checking other types
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
        foreach (EnemyWaveInfo enemyInfo in currentWave.enemies)
        {
            spawnStates.Add(new SpawnState(enemyInfo));
        }

        UpdateCurrentDayAndWave(); //sync indexes and classes and trigger banner animation
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