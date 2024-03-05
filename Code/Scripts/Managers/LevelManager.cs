using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager main;

    [Header("Attributes")]
    [SerializeField] private int startingCoins ;
    [SerializeField] public Scenario currentScenario;
    [SerializeField] public int currentDayIndex = 0;
    [SerializeField] public int currentWaveIndex = 0;

    public Transform startPoint;
    public Transform[] path;

    public int coins;
    private static int gameSpeed = 1; // Default game speed

    // Public method to set game speed
    public static void SetGameSpeed(int speed)
    {
        gameSpeed = speed;
    }

    // Public method to get current game speed
    public static int GetGameSpeed()
    {
        return gameSpeed;
    }

    private void Awake()
    {
        if (main != null && main != this)
        {
            Destroy(gameObject); // Ensure singleton integrity
            return;
        }
        main = this;
        DontDestroyOnLoad(gameObject); // Persist across scenes
    }

    private void Start(){
        coins = startingCoins;
    }

    public void IncreaseCurrency( int amount ){
        coins += amount;
    }

    public bool SpendCurrency( int amount ){
        if (amount <= coins){
            //Buy Item
            coins -= amount;
            return true;
        }
        else{ 
            Debug.Log("Not enough money");
            return false;
        }
        
    }

    // Call this to start a scenario
    public void LoadScenario(Scenario scenario)
    {
        currentScenario = scenario;
        currentDayIndex = 0;
        currentWaveIndex = 0;
        LoadDay();
    }

    void LoadDay()
    {
        if (currentDayIndex < currentScenario.days.Count)
        {
            Day currentDay = currentScenario.days[currentDayIndex];
            Debug.Log("START DAY!");
            // Additional setup for the day can go here, e.g., initializing wave spawners
        }
        else
        {
            Debug.Log("Scenario Complete!");
            // Handle scenario completion, e.g., return to map
        }
    }

    // Call to progress to the next wave or day
    public void NextWave()
    {
        currentWaveIndex++;
        Day currentDay = currentScenario.days[currentDayIndex];

        if (currentWaveIndex >= currentDay.waves.Count)
        {
            // All waves for the day completed, load next day
            currentWaveIndex = 0;
            currentDayIndex++;
            LoadDay();
        }
        else
        {
            // Load next wave within the current day
            // Wave setup logic goes here, e.g., triggering wave spawn mechanisms
        }
    }
}
