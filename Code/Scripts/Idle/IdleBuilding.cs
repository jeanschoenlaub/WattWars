using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IdleBuilding : MonoBehaviour
{
    [Header("References")]
    [SerializeField] TextMeshProUGUI buildingCoinsUI;
    [SerializeField] Sprite onSprite;
    [SerializeField] Sprite offSprite;
    
    private bool isBuildingOn = false;
    private bool isSpotPriceMode = false;
    private bool isLockInContractMode = false;

    //For the spot price mode
    public float coins = 0;
    private float spotPriceRate = 5.0f; 
    private DateTime lastCollectTime;
    private float timeIntervalForSpotPrice = 10f; // Interval to add coins in spot price mode

    
    //For the contract mode
    private DateTime contractEndTime; 
    private bool isContractActive = false;
    private float futureContractCoins = 2;

    private Image imageComponent;

    void Start()
    {
        imageComponent = GetComponent<Image>();
        
        
        lastCollectTime = LoadLastCollectTime();

        if (isBuildingOn)
        {
            imageComponent.sprite = onSprite;
        }else {
            imageComponent.sprite = offSprite;
        }

        if (isSpotPriceMode)
        {
            UpdateCoinsBasedOnLastCollect();
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

    void Update()
    {
        if (isSpotPriceMode){
            UpdateSpotPriceMode();
        }
        if (isContractActive){
            CheckContractStatus();
        }

        buildingCoinsUI.text = coins.ToString();
    }

    // Handles spot price mode coin generation
    public void ToggleOnOff()
    {
        if (isBuildingOn){
            isSpotPriceMode = false;
            isContractActive = false;
            imageComponent.sprite = offSprite;
        }
        else if (!isBuildingOn){
            isSpotPriceMode = true;
            imageComponent.sprite = onSprite;
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
