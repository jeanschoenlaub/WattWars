using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class RewardManager : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] private GameObject mainCanvas;
    [SerializeField] private Animator cameraAnimator;
    [SerializeField] private Animator EndScenarioAnimator;
    [SerializeField] private Animator dayRewardsAnimator;
    [SerializeField] private GameObject dayRewardsTimeline;

    [Header("Manager References")]
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private BuildManager buildManager;
    [SerializeField] private ShopManager shopManager;

    [Header("Sprites")]
    [SerializeField] private Sprite timelineDay1;
    [SerializeField] private Sprite timelineDay2;
    [SerializeField] private Sprite timelineDay3;
    [SerializeField] private Sprite timelineDay4;
    
    [Header("Card Rewards Attribute")]
    [SerializeField] private Sprite damageSprite;
    [SerializeField] private Sprite costSprite;
    [SerializeField] private Image card1TowerImage;
    [SerializeField] private Image card1IconImage;
    [SerializeField] private TextMeshProUGUI card1Text;
    [SerializeField] private Image card2TowerImage;
    [SerializeField] private Image card2IconImage;
    [SerializeField] private TextMeshProUGUI card2Text;
    [SerializeField] private Image card3TowerImage;
    [SerializeField] private Image card3IconImage;
    [SerializeField] private TextMeshProUGUI card3Text;

    [Header("Weather Icons")]
    [SerializeField] private Image tomorrowWeatherImage;
    [SerializeField] private Sprite WeatherIconSunny;
    [SerializeField] private Sprite WeatherIconCloudy;
    [SerializeField] private Sprite WeatherIconOvercast;

    private struct Reward
    {
        public Structure Structure;
        public int Value;
        public string RewardType;
    }

    private List<Reward> rewards; // To store the rewards

    private System.Random random = new System.Random(); 
    public int currentGameSpeed = 1; // For resetting the same gamespeed after rewards

    //Singletons
    private AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindWithTag("Audio").GetComponent<AudioManager>();
    }

    public void EndScreenAnim() {
        mainCanvas.SetActive(false);
        cameraAnimator.SetTrigger("MoveUp");
        EndScenarioAnimator.SetTrigger("TurnOnEndScreen"); // 2 second delay on the animation for the cam to move up
    }

    public void FinishTD(){
        LevelManager.main.ExitToMainMenu(true);  // ScenarioComplete flag equal to true
    }

    public void AnimateDayReward(int dayNumber, Weather weather) 
    {
        dayRewardsAnimator.SetTrigger("FadeIn");

        currentGameSpeed = LevelManager.GetGameSpeed();
        LevelManager.SetGameSpeed(0);

        DisplayDayTimeline(dayNumber);

        rewards = RandomiseRewards();
        // foreach (var reward in rewards) {
        //     Debug.Log($"Reward: {reward.Structure.structureName}, Type: {reward.RewardType}, Value: {reward.Value}");
        // }
        UpdateRewardCards(rewards, weather);

        tomorrowWeatherImage.sprite = GetWeatherIcon(weather);
    }

    // Randomise and stores the reward in Rewards
    private List<Reward> RandomiseRewards(){

        Structure[] structures = buildManager.structures;
        List<Reward> randomisedRewards = new List<Reward>();


        for (int i = 0; i < 3; i++)
        {
            // Step 1: Select a random structure
            Structure randomStructure = structures[random.Next(structures.Length)];

            // Step 2: Get a random number from the list [30, 40, 50]
            int[] numbers = { 30, 40, 50 };
            int randomNumber = numbers[random.Next(numbers.Length)];

            // Step 3: Randomize the reward type from cost, attack
            string[] rewardTypes = { "cost", "attack" };
            string randomRewardType = rewardTypes[random.Next(rewardTypes.Length)];

            // Create a new reward and add it to the list
            Reward reward = new Reward
            {
                Structure = randomStructure,
                Value = randomNumber,
                RewardType = randomRewardType
            };

            randomisedRewards.Add(reward);
        }

        return randomisedRewards;
    }

    private void UpdateRewardCards(List<Reward> rewards, Weather weather)
    {
        if (rewards.Count < 3) return;

        // Update card 1
        card1TowerImage.sprite = rewards[0].Structure.icon;
        card1IconImage.sprite = rewards[0].RewardType == "cost" ? costSprite : damageSprite;
        card1Text.text = rewards[0].RewardType == "cost" ? $"-{rewards[0].Value}%" : $"+{rewards[0].Value}%";

        // Update card 2
        card2TowerImage.sprite = rewards[1].Structure.icon;
        card2IconImage.sprite = rewards[1].RewardType == "cost" ? costSprite : damageSprite;
        card2Text.text = rewards[1].RewardType == "cost" ? $"-{rewards[1].Value}%" : $"+{rewards[1].Value}%";

        // Update card 3
        card3TowerImage.sprite = rewards[2].Structure.icon;
        card3IconImage.sprite = rewards[2].RewardType == "cost" ? costSprite : damageSprite;
        card3Text.text = rewards[2].RewardType == "cost" ? $"-{rewards[2].Value}%" : $"+{rewards[2].Value}%";
    }

    private Sprite GetWeatherIcon(Weather weather)
    {
        return weather == Weather.Sunny ? WeatherIconSunny :
               weather == Weather.Cloudy ? WeatherIconCloudy :
               WeatherIconOvercast;
    }

    private void DisplayDayTimeline(int dayNumber){
        Image timelineImage = dayRewardsTimeline.GetComponent<Image>();
        switch(dayNumber) 
        {
            case 1:
                timelineImage.sprite = timelineDay1;
                break;
            case 2:
                timelineImage.sprite = timelineDay2;
                break;
            case 3:
                timelineImage.sprite = timelineDay3;
                break;
            case 4:
                timelineImage.sprite = timelineDay4;
                break;
            default:
                Debug.LogWarning("Invalid day number. No sprite assigned.");
                break;
        }
    }

    public void FinishDayReward(int cardIndex) 
    {
        Reward selectedReward = rewards[cardIndex];
        UpdateStructures(selectedReward);
        Debug.Log(selectedReward.Structure + selectedReward.RewardType + selectedReward.Value);
        dayRewardsAnimator.SetTrigger("FadeOut");
        LevelManager.SetGameSpeed(currentGameSpeed);
        waveManager.StartNextWave(newDay: true);
    }

    private void UpdateStructures(Reward selectedReward){
        foreach (var structure in buildManager.structures)
        {
            if (structure == selectedReward.Structure)
            {
                if (selectedReward.RewardType == "cost")
                {
                    float discountFactor = 1 - (float)selectedReward.Value / 100;
                    float newCostFloat = discountFactor * structure.currentCost;

                    Debug.Log("new cost:"+newCostFloat);

                    // Decrease cost to the nearest integer
                    structure.UpdateCost((int)Mathf.Round(newCostFloat));
                }
                else if (selectedReward.RewardType == "attack")
                {
                    Debug.Log("not implemented yet");
                }
                break;
            }
        }
        shopManager.SetupShopUI(); // restes shop anager with right price
    }
}