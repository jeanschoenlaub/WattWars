using UnityEngine;
using UnityEngine.SceneManagement;

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
    private static int savedGameSpeed = 1; // To save the speed before pausing
    private AudioManager audioManager; 

    private void Awake()
    {
        if (main != null && main != this)
        {
            Destroy(gameObject); // Ensure singleton integrity
            return;
        }
        main = this;
        //DontDestroyOnLoad(gameObject); // Persist across scenes
        audioManager = GameObject.FindWithTag("Audio").GetComponent<AudioManager>();
    }

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

    // Method to pause the game (set speed to 0)
    public static void PauseGame()
    {
        savedGameSpeed = gameSpeed; // Save the current speed
        SetGameSpeed(0); // Pause the game
    }

    // Method to resume the game at previously saved speed
    public static void ResumeGame()
    {
        SetGameSpeed(savedGameSpeed); // Resume at saved speed
    }

    // Load Main Menu
    public void ExitToMainMenu(bool ScenarioComplete){

        

        // If called with level complete flag && this is the highest level unlocked we save the progression 
        if (ScenarioComplete && currentScenario.scenarioId + 1> PlayerPrefs.GetInt("UnlockedLevels")){
            PlayerPrefs.SetInt("UnlockedLevels", currentScenario.scenarioId+1);
        }

        Debug.Log("Current unlocked sc: " + PlayerPrefs.GetInt("UnlockedLevels"));

        string sceneName = "Menu";
        SceneManager.LoadScene(sceneName);
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
}
