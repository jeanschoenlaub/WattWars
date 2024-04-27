using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public static LevelManager main;

    [Header("Attributes")]
    [SerializeField] public Scenario currentScenario;
    [SerializeField] public Animator cityCoinAnimator;
    [SerializeField] public TextMeshProUGUI cityCoinAnimationText;
     

    public Transform startPoint;
    public Transform[] path;

    public int coins; 
    public int cityCoinGen = 10; 
    float timeSinceLastCityCoins = 0;

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
        gameSpeed = 1; // Reset game speed to default upon scene load
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

    // Method to pause the game (set speed to 0)
    public static void SaveGameSpeed()
    {
        savedGameSpeed = gameSpeed; // Save the current speed
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
            PlayerPrefs.SetInt("CompletedLevels", currentScenario.scenarioId);
        }

        string sceneName = "Menu";
        SceneManager.LoadScene(sceneName);
    }


    private void Start(){
        coins =  currentScenario.Coins;
        numberOfLives = currentScenario.Lives;
    }

    private void Update() {
        timeSinceLastCityCoins += Time.deltaTime * gameSpeed;
        if (timeSinceLastCityCoins >  5f)
        {
            IncreaseCurrency(cityCoinGen); // Increase coins by 1, adjust this value as needed
            cityCoinAnimationText.text = cityCoinGen.ToString();
            cityCoinAnimator.SetTrigger("CoinAppear");
            timeSinceLastCityCoins = 0f;
        }
    }

    public void IncreaseCurrency( int amount ){
        coins += amount;
        cityCoinAnimationText.text = amount.ToString();
        cityCoinAnimator.SetTrigger("CoinAppear");
    }

    public void DecreaseLives( int amount ){
        numberOfLives -= amount;
        if (numberOfLives<0){
            EndOfGame();
        }
    }

    public void EndOfGame() { 
        SceneManager.LoadScene("Menu");
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

    public int GetCurrentMoney(){
        return coins;
    }
}
