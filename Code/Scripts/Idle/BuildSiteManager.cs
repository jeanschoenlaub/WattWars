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
    private DateTime lastCollectTime;
    private DateTime buildingEndTime;

    private Image imageComponent; //used to access the image of building
    private bool isBuildingOn = false;

    void Start()
    {
        imageComponent = constructionField.GetComponent<Image>();
        lastCollectTime = LoadLastCollectTime();

        productionGoUI.SetActive(isBuildingOn);
        constructionGoUI.SetActive(isUpgrading);

        LoadBuildingType();

        if (!hasBuilding){
            imageComponent.sprite = constructionFieldSprite;
        }
        else if (hasBuilding && isBuildingOn)
        {
            imageComponent.sprite = buildingType.onSprite;
            UpdateCoinsBasedOnLastCollect();
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
        SaveBuildingType(null);
    }

    public void CollectCoins()
    {
        IdleManager.main.IncreaseCurrency(coins);
        coins=0;
        productionGoUI.SetActive(false);
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

    // Save method for building end time
    private void SaveBuildingEndTime(DateTime endTime)
    {
        PlayerPrefs.SetString("Building" + buildingId + "_EndTime", endTime.ToString());
        PlayerPrefs.Save();
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
    }

    // Handles spot price mode coin generation
    private void UpdateSpotPriceMode()
    {
        // Calculate time since last update and update coins accordingly
        TimeSpan elapsed = DateTime.Now - lastCollectTime;
        if (elapsed.TotalSeconds >= timeIntervalForSpotPrice)
        {
            int intervalsPassed = (int)(elapsed.TotalSeconds / timeIntervalForSpotPrice);
            int coinsToAdd = (int)Math.Ceiling(intervalsPassed * spotPriceRate);
            coins += coinsToAdd;
            lastCollectTime = DateTime.Now;
            SaveLastCollectTime(lastCollectTime);
        }

        // Deactivates the construction UI and shows production UI to show coins generated
        
        if (coins > 0){
            productionGoUI.SetActive(true);
        }
        constructionGoUI.SetActive(false);
        productionCoinsTextUI.text = coins.ToString();
    }

    // Save and load methods for persisting data
    private void SaveLastCollectTime(DateTime time)
    {
        PlayerPrefs.SetString("LastCollectTime", time.ToString());
        PlayerPrefs.Save();
    }

    // Save and load methods for persisting data
    private void SaveBuildingType(IdleBuilding buildingtype)
    {
        PlayerPrefs.SetString("Building" + buildingId + "Type", buildingtype.buildingName);
        PlayerPrefs.Save();
    }

    private void LoadBuildingType()
    {
        string buildingName = PlayerPrefs.GetString("Building" + buildingId + "Type", "");
        Debug.Log(buildingName);
        if (!string.IsNullOrEmpty(buildingName))
        {
            if (buildingDatabase != null)
            {
                buildingType = buildingDatabase.GetBuildingByName(buildingName);
                if (buildingType != null)
                {
                    hasBuilding = true;
                }
                else
                {
                    Debug.LogError("Building type not found in database");
                }
            }
            else
            {
                Debug.LogError("BuildingDatabase is not assigned");
            }
        }
    }


    private DateTime LoadLastCollectTime()
    {
        string timeStr = PlayerPrefs.GetString("LastCollectTime", DateTime.Now.ToString());
        return DateTime.Parse(timeStr);
    }

    private void UpdateCoinsBasedOnLastCollect()
    {
        TimeSpan timeSinceLast = DateTime.Now - lastCollectTime;
        int missedIntervals = (int)(timeSinceLast.TotalSeconds / timeIntervalForSpotPrice);
        int coinsToAdd = (int)Math.Ceiling(missedIntervals * spotPriceRate);
        coins += coinsToAdd;
        lastCollectTime = DateTime.Now; // Reset last collect time
        SaveLastCollectTime(lastCollectTime);
    }

    // Optionally, a method to get the remaining production time to update the UI
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
