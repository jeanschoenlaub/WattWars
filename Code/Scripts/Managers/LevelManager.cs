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
    private AudioManager audioManager; 

    private void Awake()
    {
        if (main != null && main != this)
        {
            Destroy(gameObject); // Ensure singleton integrity
            return;
        }
        main = this;
        DontDestroyOnLoad(gameObject); // Persist across scenes
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
