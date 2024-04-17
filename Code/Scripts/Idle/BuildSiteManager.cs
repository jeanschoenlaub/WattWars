using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildSiteManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] TextMeshProUGUI buildingCoinsUI;
    [SerializeField] Sprite constructionFieldSprite;
    [SerializeField] GameObject constructionField;
    [SerializeField] UpgradePopUp upgradePopUp;
    [SerializeField] BuildPopUp buildPopUp;

    private Image imageComponent; // We get the image component programaticaly

    // For saving and loading
    public int buildingId;
    
    //For the construction mode
    public bool hasBuilding;
    public bool isUpgrading; // Or Building
    public DateTime buildingEndTime;
    public IdleBuilding currentlyBuildingBuildingType;
    public IdleBuilding buildingType;

    //For the spot price mode
    private bool isBuildingOn = false;
    private bool isSpotPriceMode = false;
    public float coins = 0;
    private float spotPriceRate = 5.0f; 
    private DateTime lastCollectTime;
    private float timeIntervalForSpotPrice = 10f; // Interval to add coins in spot price mode
    
    
    //For the contract mode
    private DateTime contractEndTime; 
    private bool isContractActive = false;
    private float futureContractCoins = 2;
    private bool isLockInContractMode = false;

    void Start()
    {
        imageComponent = constructionField.GetComponent<Image>();
        
        lastCollectTime = LoadLastCollectTime();

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

        if (isSpotPriceMode)
        {
            UpdateCoinsBasedOnLastCollect();
        }
    }

    void Update()
    {
        CheckConstructionCompletion();
        if (isSpotPriceMode){
            UpdateSpotPriceMode();
        }
        if (isContractActive){
            CheckContractStatus();
        }

        buildingCoinsUI.text = coins.ToString();
    }

    void CheckConstructionCompletion()
    {
        if (isUpgrading){
            buildingCoinsUI.text = GetRemainingTime();
            if (buildingEndTime > DateTime.MinValue && DateTime.Now >= buildingEndTime)
            {
                CompleteConstruction();
            }
        }
    }

    void CompleteConstruction()
    {
        isUpgrading = false;
        hasBuilding = true;
        buildingType = currentlyBuildingBuildingType;

        imageComponent.sprite = buildingType.offSprite; 
        
        buildingEndTime = DateTime.MinValue;
        SaveBuildingEndTime(buildingEndTime); // Clear the saved end time
    }

    public void ManagePopUp(){
        if (hasBuilding){
            upgradePopUp.TogglePopUp();
        }
        else{
            buildPopUp.TogglePopUp();
        }
    }

    // Save and load methods for building end time
    private void SaveBuildingEndTime(DateTime endTime)
    {
        PlayerPrefs.SetString("Building" + buildingId + "_EndTime", endTime.ToString());
        PlayerPrefs.Save();
    }

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

    // Method to start a lock-in contract
    public void StartContract(float durationInSeconds, float contractCoins)
    {
        if (!isContractActive)
        {
            futureContractCoins = contractCoins;
            contractEndTime = DateTime.Now.AddSeconds(durationInSeconds);
            isContractActive = true;
            isSpotPriceMode = false;
        }
    }

    // Handles spot price mode coin generation
    public void ToggleOnOff()
    {
        if (isBuildingOn){
            isSpotPriceMode = false;
            isContractActive = false;
            imageComponent.sprite = buildingType.offSprite;
        }
        else if (!isBuildingOn){
            isSpotPriceMode = true;
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
            coins += intervalsPassed * spotPriceRate;
            lastCollectTime = DateTime.Now;
            SaveLastCollectTime(lastCollectTime);
        }
    }

    // Save and load methods for persisting data
    private void SaveLastCollectTime(DateTime time)
    {
        PlayerPrefs.SetString("LastCollectTime", time.ToString());
        PlayerPrefs.Save();
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
        coins += missedIntervals * spotPriceRate;
        lastCollectTime = DateTime.Now; // Reset last collect time
        SaveLastCollectTime(lastCollectTime);
    }

    // Checks if the contract is still active
    private void CheckContractStatus()
    {
        if (isContractActive && DateTime.Now >= contractEndTime)
        {
            isContractActive = false;
            coins += futureContractCoins;
        }
    }

    // Optionally, a method to get the remaining production time to update the UI
    public string GetRemainingTime()
    {
        if (isUpgrading)
        {
            TimeSpan remaining = buildingEndTime - DateTime.Now;
            if (remaining.Ticks < 0)
                remaining = TimeSpan.Zero; // Ensure we don't display negative time
            return "a";
            //return string.Format("{0:D2}:{1:D2}:{2:D2}", remaining.Hours, remaining.Minutes, remaining.Seconds);
        }
        if (isContractActive)
        {
            TimeSpan remaining = contractEndTime - DateTime.Now;
            if (remaining.Ticks < 0)
                remaining = TimeSpan.Zero; // Ensure we don't display negative time

            return string.Format("{0:D2}:{1:D2}:{2:D2}", remaining.Hours, remaining.Minutes, remaining.Seconds);
        }
        return "00:00:00";
    }
}
