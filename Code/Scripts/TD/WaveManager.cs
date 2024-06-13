using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WaveManager : MonoBehaviour {

    [Header("References")]
    [SerializeField] private Animator newWaveAnimator;
    [SerializeField] private RewardManager rewardManager;

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
        currentDay = LevelManager.main.currentScenario.days[currentDayIndex];
        StartFirstWave(); //Special case for beginning
    }

    // Function to take sync Day and wave index and class and trigger banner animation
    private void UpdateCurrentDayAndWave()
    {
        if (LevelManager.main.currentScenario.days.Count > currentDayIndex)
        {     
            //Just next wave on the same day
            if (currentDay.waves.Count > currentWaveIndex + 1)
            {
                currentWaveIndex++;
                // Also if this is a night wave --> Fade to night
                // To-Do if multiple night waves change logic
                if (currentDay.waves[currentWaveIndex].night == true){
                    WeatherManager.main.ChangeToNight();
                }
            }
            
            // New Day 
            else if (currentDay.waves.Count == currentWaveIndex +1){
                currentDayIndex = currentDayIndex +1;
                currentWaveIndex = 0;

                WeatherManager.main.ResetSunPosition();
                WeatherManager.main.ChangeToDay();
                WeatherManager.main.UpdateWeather(LevelManager.main.currentScenario.days[currentDayIndex]);
            }
            currentDay = LevelManager.main.currentScenario.days[currentDayIndex];
            currentWave = currentDay.waves[currentWaveIndex];
            TriggerWaveBannerAnimation();
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

            // If timer up AND another wave in the same day --> Next wave
            if (timeSinceLastWave > timeBetweenWaves && currentWaveIndex + 1 < currentDay.waves.Count )
            {
                if (!isTutorialOn){
                    UpdateCurrentDayAndWave();
                    StartNextWave();
                    timeSinceLastWave = 0; //Reset the counter
                }
                // Special case for tutorial we only spawn next wave if all enemies dead
                else if (isTutorialOn){
                    if (enemiesAlive == 0){
                        UpdateCurrentDayAndWave();
                        StartNextWave();
                        timeSinceLastWave = 0; //Reset the counter
                    }
                }
            }
            // If timer up AND last wave of day AND not last day--> Next Day OR Menu
            else if ( timeSinceLastWave > timeBetweenWaves && currentWaveIndex + 1 == currentDay.waves.Count && LevelManager.main.currentScenario.days.Count > currentDayIndex + 1)
            {
                UpdateCurrentDayAndWave(); //sync indexes and classes and trigger banner animation
                rewardManager.startRewardTransition();
                Debug.Log("StartTransition");
                //StartNextWave();
                timeSinceLastWave = 0; //Reset the counter
            }
            // If all ennemy dead and last wave --> Menu
            else if (enemiesAlive == 0 && currentWaveIndex + 1 == currentDay.waves.Count && LevelManager.main.currentScenario.days.Count == currentDayIndex + 1){
                LevelManager.main.ExitToMainMenu(true);  // ScenarioComplete flag equal to true
            }
        }
    }

    private void StartFirstWave()
    {
        // First we set current day and wave 
        currentDay = LevelManager.main.currentScenario.days[0];
        currentWave = currentDay.waves[0];

        //Set the spawn states
        spawnStates.Clear();
        foreach (EnemyWaveInfo enemyInfo in currentWave.enemies)
        {
            spawnStates.Add(new SpawnState(enemyInfo));
        }

        // Then we trigger animations and update the weather
        TriggerWaveBannerAnimation();
        WeatherManager.main.UpdateWeather(LevelManager.main.currentScenario.days[currentDayIndex]);
    }

    private void StartNextWave()
    {
        spawnStates.Clear();

        foreach (EnemyWaveInfo enemyInfo in currentWave.enemies)
        {
            spawnStates.Add(new SpawnState(enemyInfo));
        }
    }

    private void SpawnEnemy(GameObject enemyPrefab)
    {
        // Randomize y a bit
        Vector3 originalPosition = LevelManager.main.startPoint.position;
        float randomOffsetY = Random.Range(-0.4f, 0.5f);
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
        enemiesAlive--;
    }
}