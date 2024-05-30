using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildSiteManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Sprite constructionFieldSprite;
    [SerializeField] private GameObject constructionGoUI;
    [SerializeField] private TextMeshProUGUI constructionTimeTextUI;
    [SerializeField] private GameObject productionGoUI;
    [SerializeField] private TextMeshProUGUI productionCoinsTextUI;
    [SerializeField] private GameObject constructionField;

    [Header("Pop ups References")]
    [SerializeField] private UpgradePopUp upgradePopUp;
    [SerializeField] private BuildPopUp buildPopUp;

    [Header("Building Configuration")]
    [SerializeField] private BuildingDatabase buildingDatabase; // Assign this in the Inspector
    public int buildingId;
    public bool hasBuilding;
    public bool isUpgrading;
    public IdleBuilding currentlyBuildingBuildingType;
    public IdleBuilding buildingType;

    [Header("Financial Management")]
    public int coins = 0;
    private float spotPriceRate = 5.0f;
    private float timeIntervalForSpotPrice = 10f; 

    [Header("Time Management")]
    private DateTime lastTimeCoinsGenerated;
    private DateTime buildingEndTime;

    private Image imageComponent; //used to access the image of building
    private bool isBuildingOn = false;

    void Start()
    {
        imageComponent = constructionField.GetComponent<Image>();
        
        // First we load the building state
        LoadBuildingType();
        LoadBuildingState();
        LoadBuildingCoins(); //Updates with coins generated offline
        LoadBuildingEndTime();

        // Then we set the building UI to correspond to the state
        productionGoUI.SetActive(isBuildingOn);
        constructionGoUI.SetActive(isUpgrading);

        if (!hasBuilding){
            imageComponent.sprite = constructionFieldSprite;
        }
        else if (hasBuilding && isBuildingOn)
        {
            imageComponent.sprite = buildingType.onSprite;
        }
        else if (!isBuildingOn)
        {
            imageComponent.sprite = buildingType.offSprite;
        }
    }

    void Update()
    {
        if (isUpgrading){
            CheckConstructionCompletion();
        }
        else if (isBuildingOn){
            UpdateSpotPriceMode();
        }
    }

    void CheckConstructionCompletion()
    {
        // Deactivates the coins and shows construction UI to show time remaining
        productionGoUI.SetActive(false);
        constructionGoUI.SetActive(true);
        constructionTimeTextUI.text = GetRemainingTime();

        if (buildingEndTime > DateTime.MinValue && DateTime.Now >= buildingEndTime)
        {
            CompleteConstruction();
        }
    }

    void CompleteConstruction()
    {
        isUpgrading = false;
        hasBuilding = true;
        buildingType = currentlyBuildingBuildingType;

        imageComponent.sprite = buildingType.offSprite; 
        
        // Resets the buildEndTime
        buildingEndTime = DateTime.MinValue;
        SaveBuildingEndTime(buildingEndTime); 
        SaveBuildingType(buildingType);
    }

    public void DeleteBuilding(){
        hasBuilding = false;
        isUpgrading = false; // Assuming you want to reset this as well
        currentlyBuildingBuildingType = null; // Reset the field to null
        buildingType = null; // Reset the field to null
        imageComponent.sprite = constructionFieldSprite;
        coins = 0;
        productionGoUI.SetActive(false);
        constructionGoUI.SetActive(false);
        SaveBuildingType(null);
    }

    public void CollectCoins()
    {
        IdleManager.main.IncreaseCurrency(coins);
        coins=0;
        productionGoUI.SetActive(false);
        SaveBuildingCoins(coins);
    }

    public void ManagePopUp(){
        if (hasBuilding){
            upgradePopUp.SetCurrentBuildingManager(this);
            upgradePopUp.TogglePopUp();
        }
        else{
            buildPopUp.SetCurrentBuildingManager(this);
            buildPopUp.TogglePopUp();
        }
    }


     public void SetBuilding(IdleBuilding building)
    {
        if (IdleManager.main.GetCurrentMoney() >= building.cost)
        {
            IdleManager.main.SpendCurrency(building.cost);
            buildingEndTime = DateTime.Now.AddSeconds(building.timeToBuild);
            currentlyBuildingBuildingType = building;
            
            // Initiate construction visual state
            isUpgrading = true;
            imageComponent.sprite = constructionFieldSprite;
        }
    }


    // Handles spot price mode coin generation
    public void ToggleOnOff()
    {
        if (isBuildingOn){
            isBuildingOn = false;
            imageComponent.sprite = buildingType.offSprite;
        }
        else if (!isBuildingOn){
            isBuildingOn = true;
            imageComponent.sprite = buildingType.onSprite;
        }
        SaveBuildingState(isBuildingOn);
    }

    // Handles spot price mode coin generation
    private void UpdateSpotPriceMode()
    {
        // Calculate time since last update and update coins accordingly
        TimeSpan elapsed = DateTime.Now - lastTimeCoinsGenerated;
        if (elapsed.TotalSeconds >= timeIntervalForSpotPrice)
        {
            int intervalsPassed = (int)(elapsed.TotalSeconds / timeIntervalForSpotPrice);
            int coinsToAdd = (int)Math.Ceiling(intervalsPassed * spotPriceRate);
            coins += coinsToAdd;
            lastTimeCoinsGenerated = DateTime.Now;
            SaveLastTimeCoinsGenerated(lastTimeCoinsGenerated);
            SaveBuildingCoins(coins);
        }

        // Deactivates the construction UI and shows production UI to show coins generated
        if (coins > 0){
            productionGoUI.SetActive(true);
        }
        constructionGoUI.SetActive(false);
        productionCoinsTextUI.text = coins.ToString();
    }

    // Save method for building end time
    private void SaveBuildingEndTime(DateTime endTime)
    {
        PlayerPrefs.SetString("Building" + buildingId + "_EndTime", endTime.ToString());
        PlayerPrefs.Save();
    }

    // Save method for building end time
    private void SaveBuildingState( bool isBuildingOn)
    {
        PlayerPrefs.SetString("Building" + buildingId + "_isBuildingOn", isBuildingOn.ToString());
        PlayerPrefs.Save();
    }

    private void LoadBuildingState()
    {
        // Retrieve the string and convert it to boolean
        string key = "Building" + buildingId + "_isBuildingOn";
        string value = PlayerPrefs.GetString(key, "false"); // Default to "false" if the key does not exist
        isBuildingOn = bool.Parse(value);
    }

    private void SaveBuildingCoins(int coins)
    {
        PlayerPrefs.SetInt("Building" + buildingId + "_Coins", coins);
        PlayerPrefs.Save();
    }

    private void LoadBuildingCoins()
    {
        // Get any uncollected coins before the game was shut
        coins = PlayerPrefs.GetInt("Building" + buildingId + "_Coins", 0); // Default to 0 if no coins have been saved yet

        //Then we add coins generaterd offline 
        LoadLastTimeCoinsGenerated(); // Fetch and update lastTimeCoinsGenerated
        if (isBuildingOn){
            UpdateSpotPriceMode();
        }
    }

    // Load method for building end time
    private DateTime LoadBuildingEndTime()
    {
        string key = "Building" + buildingId + "_EndTime";
        string endTimeStr = PlayerPrefs.GetString(key, "");
        if (!string.IsNullOrEmpty(endTimeStr))
        {
            return DateTime.Parse(endTimeStr);
        }
        return DateTime.MinValue; // Return min value if nothing is saved
    }

    // Save time using ISO 8601 
    private void SaveLastTimeCoinsGenerated(DateTime time)
    {
        string timeString = time.ToString("o"); // "o" denotes a round-trip format that includes the timezone.
        PlayerPrefs.SetString("Building" + buildingId + "LastTimeCoinsGenerated", timeString);
        PlayerPrefs.Save();
    }

    // Save and load methods for persisting data
    private void LoadLastTimeCoinsGenerated()
    {
        string timeString = PlayerPrefs.GetString("Building" + buildingId + "LastTimeCoinsGenerated", DateTime.Now.ToString("o"));
        lastTimeCoinsGenerated = DateTime.Parse(timeString, null, System.Globalization.DateTimeStyles.RoundtripKind);
    }

    // Save building type to keep track accross
    private void SaveBuildingType(IdleBuilding buildingtype)
    {
        // Check if buildingtype is null and handle appropriately
        if (buildingtype != null)
        {
            PlayerPrefs.SetString("Building" + buildingId + "Type", buildingtype.buildingName);
        }
        else
        {
            // Set a default or empty value to indicate no building
            PlayerPrefs.SetString("Building" + buildingId + "Type", "");
        }
        PlayerPrefs.Save();
    }

    private void LoadBuildingType()
    {
        string buildingName = PlayerPrefs.GetString("Building" + buildingId + "Type", "");
        if (!string.IsNullOrEmpty(buildingName))
        {
            if (buildingDatabase != null)
            {
                buildingType = buildingDatabase.GetBuildingByName(buildingName);
                {
                    hasBuilding = true;
                }
            }
        }
    }

    // Method to get the remaining production time to update the UI
    public string GetRemainingTime()
    {
        if (isUpgrading)
        {
            TimeSpan remaining = buildingEndTime - DateTime.Now;
            if (remaining.Ticks < 0)
                remaining = TimeSpan.Zero; // Ensure we don't display negative time
            return string.Format("{0:D2}:{1:D2}:{2:D2}", remaining.Hours, remaining.Minutes, remaining.Seconds);
        }
        return "00:00:00";
    }
}
