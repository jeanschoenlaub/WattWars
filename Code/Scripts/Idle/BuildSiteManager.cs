using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildSiteManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Sprite constructionFieldSprite;
    [SerializeField] private GameObject constructionField;
    [SerializeField] private UpgradePopUp upgradePopUp;
    [SerializeField] private BuildPopUp buildPopUp;
    [SerializeField] private GameObject constructionGoUI;
    [SerializeField] private TextMeshProUGUI constructionTimeTextUI;
    [SerializeField] private GameObject productionGoUI;
    [SerializeField] private TextMeshProUGUI productionCoinsTextUI;


    [Header("Building Configuration")]
    public int buildingId;
    public bool hasBuilding;
    public bool isUpgrading;
    public IdleBuilding currentlyBuildingBuildingType;
    public IdleBuilding buildingType;

    [Header("Financial Management")]
    public float coins = 0;
    private float spotPriceRate = 5.0f;
    private float timeIntervalForSpotPrice = 10f; 

    [Header("Time Management")]
    private DateTime lastCollectTime;
    private DateTime buildingEndTime;
    private DateTime contractEndTime;

    private Image imageComponent;
    private bool isBuildingOn = false;
    private bool isSpotPriceMode = false;

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
        if (isUpgrading){
            CheckConstructionCompletion();
        }
        else if (isSpotPriceMode){
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
    }

    public void ManagePopUp(){
        if (hasBuilding){
            upgradePopUp.TogglePopUp();
        }
        else{
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
            isSpotPriceMode = false;
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

        // Deactivates the construction UI and shows production UI to show coins generated
        productionGoUI.SetActive(true);
        constructionGoUI.SetActive(false);
        productionCoinsTextUI.text = coins.ToString();
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
