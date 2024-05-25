using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BreakerBoxManager : MonoBehaviour
{
    public Button[] Level1SelectionButtons;
    public GameObject[] RewardBoards;
    public GameObject[] Scenarios;
    public GameObject[] RewardTowers;
    public Sprite onImage;
    public Sprite offImage;
    public Sprite rewardGreenImage;
    public Sprite rewardRedImage;

    private Animator rewardAnimator;

    private void Awake() {
        UpdateLevelButtons();
    }

    private void Start() {
    }
    
    public void OpenScenario(int scenarioId){
        string sceneName = "Scenario"+scenarioId;
        SceneManager.LoadScene(sceneName);
    }

    void UpdateLevelButtons() {

        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevels", 1);
        int completedLevels = PlayerPrefs.GetInt("CompletedLevels", 0); // Get the number of completed levels

        for (int i = 0; i < Level1SelectionButtons.Length; i++) {
            // If the levels are completed we show it visually
            if (i < completedLevels) {
                Level1SelectionButtons[i].GetComponent<Image>().sprite = onImage;
                RewardBoards[i].GetComponent<Image>().sprite = rewardGreenImage;
                RewardTowers[i].GetComponent<Image>().color = Color.white;
            }
            else { 
                Level1SelectionButtons[i].GetComponent<Image>().sprite = offImage; 
                RewardBoards[i].GetComponent<Image>().sprite = rewardRedImage;
                RewardTowers[i].GetComponent<Image>().color = Color.black;
            }

            // If the levels are locked we disable them (can be unlocked and completed or not)
            if (i < unlockedLevel) {Level1SelectionButtons[i].interactable = true;}
            else {Level1SelectionButtons[i].interactable = false;}

            //If a level has been completed for the first time, trigger reward
            if (PlayerPrefs.GetInt("UnlockedLevelAnimation", 1) != 0){
                AnimateReward(completedLevels-1);
            }
        }
    }

    public void ResetProgress() {
        // Resetting level progression
        PlayerPrefs.SetInt("UnlockedLevels", 1);
        PlayerPrefs.SetInt("CompletedLevels", 0);
        PlayerPrefs.Save();
        UpdateLevelButtons();

        // Resetting Idle coins
        PlayerPrefs.SetInt("PlayerCoins", 0);
        IdleManager.main.LoadPlayerCoins();
    }

    public void BackToLevelSelection(){
        SceneManager.LoadScene("LvlSelection");
    }

    public void BackToMap(){
        SceneManager.LoadScene("Map");
    }

    public void BackToMainMenu(){
        SceneManager.LoadScene("MainMenu");
    }

    public void AnimateReward(int buttonIndex){
        
        rewardAnimator = Scenarios[buttonIndex].GetComponent<Animator>();
        rewardAnimator.enabled = true; // Enable the Animator component

        rewardAnimator.SetTrigger("Test");
    }

    public void TestRestFlag(){
        PlayerPrefs.SetInt("UnlockedLevelAnimation", 1);
        UpdateLevelButtons();
    }
}

