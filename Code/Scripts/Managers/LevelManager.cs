using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    public static LevelManager main;

    [Header("Attributes")]
    [SerializeField] private int startingCoins ;
    [SerializeField] public Scenario currentScenario;
    [SerializeField] public Animator cityCoinAnimator;
     

    public Transform startPoint;
    public Transform[] path;

    public int coins; 
    public int numberOfLives; // Taken from the Scenario SO
    private static int gameSpeed = 1; // Default game speed
    private static int savedGameSpeed = 1; // To save the speed before pausing

    private void Awake()
    {
        if (main != null && main != this)
        {
            Destroy(gameObject); // Ensure singleton integrity
            return;
        }
        main = this;
        //DontDestroyOnLoad(gameObject); // Persist across scenes
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
        coins =  currentScenario.Coins;
        numberOfLives = currentScenario.Lives;
        StartCoroutine(AddCoinsAtIntervals(5f));
    }

    public void IncreaseCurrency( int amount ){
        coins += amount;
    }

    public void DecreaseLives( int amount ){
        numberOfLives -= amount;
    }

    public int GetNumeberOfLives( ){
       return numberOfLives;
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

    IEnumerator AddCoinsAtIntervals(float interval)
    {
        while (true) // Creates an infinite loop, which is fine in a coroutine with yielding
        {
            yield return new WaitForSeconds(interval); // Wait for the specified interval
            if (gameSpeed > 0) // Only add coins if the game is not paused
            {
                IncreaseCurrency(10); // Increase coins by 1, adjust this value as needed
                cityCoinAnimator.SetTrigger("CoinAppear");
            }
        }
    }
}
