using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    public static LevelManager main;

    [Header("Attributes")]
    [SerializeField] public Scenario currentScenario;
    [SerializeField] public Animator cityCoinAnimator;
    [SerializeField] public TextMeshProUGUI cityCoinAnimationText;
    [SerializeField] public GameObject dangerEnemyTouchGrid;

    [Header("--------- Score and UI ---------")]
    [SerializeField] TextMeshProUGUI currencyUI;
    [SerializeField] TextMeshProUGUI livesUI;
    [SerializeField] TextMeshProUGUI gameSpeedText;
     
    public Transform startPoint;
    public Transform[] path;

    public int coins; 
    public int cityCoinGen = 10; 
    float timeSinceLastCityCoins = 0;

    public int numberOfLives; // Taken from the Scenario SO
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
        gameSpeed = 1; // Reset game speed to default upon scene load
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

    // Method to pause the game (set speed to 0)
    public static void SaveGameSpeed()
    {
        savedGameSpeed = gameSpeed; // Save the current speed
    }

    // Method to resume the game at previously saved speed
    public static void ResumeGame()
    {
        // TO-DO proper fix of game speed issue
        if (savedGameSpeed == 2){SetGameSpeed(savedGameSpeed);}
        else { SetGameSpeed(1);} 
    }

    // Load Main Menu
    public void ExitToMainMenu(bool ScenarioComplete){

        // If called with level complete flag && this is the highest level unlocked we save the progression 
        if (ScenarioComplete && currentScenario.scenarioId + 1> PlayerPrefs.GetInt("UnlockedLevels")){
            PlayerPrefs.SetInt("UnlockedLevelAnimation", currentScenario.scenarioId);
            PlayerPrefs.SetInt("UnlockedLevels", currentScenario.scenarioId+1);
            PlayerPrefs.SetInt("CompletedLevels", currentScenario.scenarioId);
        }
        audioManager.StartMusicForVillageMode();
        string sceneName = "LvlSelection";
        SceneManager.LoadScene(sceneName);
    }

    private void Start(){
        coins =  currentScenario.Coins;
        numberOfLives = currentScenario.Lives;
        audioManager.StartMusicForTDMode();
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
        UpdateUI();
    }

    private void UpdateUI(){
            currencyUI.text = coins.ToString();
            livesUI.text = GetNumeberOfLives().ToString();

            if (gameSpeed ==1){
                gameSpeedText.text = "x1";
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
        }else
        {
            StartCoroutine(ChangeDangerGridColor());
        }
    }

    private IEnumerator ChangeDangerGridColor()
    {
        SpriteRenderer spriteRenderer = dangerEnemyTouchGrid.GetComponent<SpriteRenderer>();

        // Change color to red with no transparency before changing back after 1 sec
        spriteRenderer.color = new Color(0f, 0f, 0f, 0.55f);
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = new Color(1f, 0f, 0f, 1f);
        yield return new WaitForSeconds(1f);
        spriteRenderer.color = new Color(0f, 0f, 0f, 0.55f);
    }

    public void EndOfGame() { 
        audioManager.StartMusicForVillageMode();
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

    public void ToggleGameSpeed(){
        audioManager.playButtonClickSFX();
        if (gameSpeed == 1){
            SetGameSpeed(2);
            gameSpeedText.text = "x2";
        } else if (gameSpeed == 2){
            SetGameSpeed(1);
            gameSpeedText.text = "x1";
        }
    }

    public int GetCurrentMoney(){
        return coins;
    }
}
