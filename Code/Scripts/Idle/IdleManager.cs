using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IdleManager : MonoBehaviour
{
    public static IdleManager main;

    [Header("Attributes")]
    [SerializeField] public TextMeshProUGUI idleCoinText;


    [SerializeField] public Sprite[] Maps;
    [SerializeField] public GameObject MapBackground;

    // For animating building  
    [SerializeField] public Transform[] buildingPositions;
    [SerializeField] public GameObject BuildingAnimGO;
     
    public int coins; 
    
    private void Awake()
    {
        if (main != null && main != this)
        {
            Destroy(gameObject); // Ensure singleton integrity
            return;
        }
        main = this;
    }


    private void Start(){
        LoadPlayerCoins();
        LoadMap();
        idleCoinText.text = FormatNumber(coins);
    }

    public static string FormatNumber(int num)
    {
        if (num >= 1000000000)
            return (num / 1000000000D).ToString("0.#") + "B"; // Billion
        if (num >= 1000000)
            return (num / 1000000D).ToString("0.#") + "M"; // Million
        if (num >= 1000)
            return (num / 1000D).ToString("0.#") + "K"; // Thousand

        return num.ToString(); // Less than 1000
    }

    public void IncreaseCurrency(int amount ){
        coins += amount;
        idleCoinText.text = FormatNumber(coins);
        SaveCurrentCoins(coins);
    }

    private void SaveCurrentCoins(int coins)
    {
        PlayerPrefs.SetInt("PlayerCoins", coins);
        PlayerPrefs.Save();
    }

    public void LoadPlayerCoins()
    {
        coins = PlayerPrefs.GetInt("PlayerCoins", 0);
    }

    //Based on what the player as unlocked, load a different map
    public void LoadMap()
    {
        int completedLvls = PlayerPrefs.GetInt("CompletedLevels", 0); // Get the number of completed levels
        MapBackground.GetComponent<Image>().sprite = Maps[completedLvls];

        int BuildingAinmationLoc = PlayerPrefs.GetInt("UnlockedLevelAnimation", 0); 

        if (BuildingAinmationLoc != 0){
            TriggerBuildingAnimation(BuildingAinmationLoc);
            PlayerPrefs.SetInt("UnlockedLevelAnimation", 0); // Reset the flag 
        }
    }

    public void TriggerBuildingAnimation(int locationIndex){
        BuildingAnimGO.SetActive(true);
        Animator buildingAnimator = BuildingAnimGO.GetComponent<Animator>();

        // Position the BuildingAnimGO at the specified location
        BuildingAnimGO.transform.position = buildingPositions[locationIndex-1].position;

        buildingAnimator.SetTrigger("Building");
    }

    public bool SpendCurrency( int amount ){
        if (amount <= coins){
            //Buy Item
            coins -= amount;
            SaveCurrentCoins(coins);
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

    public void GoToLevelSelection(){
        SceneManager.LoadScene("LvlSelection");
    }
}
