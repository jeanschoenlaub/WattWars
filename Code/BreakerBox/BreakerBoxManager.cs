using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class BreakerBoxManager : MonoBehaviour
{
    [Header("---    Attributes  ---")]
    [SerializeField] private Button[] Level1SelectionButtons;
    [SerializeField] private GameObject[] RewardBoards;
    [SerializeField] private GameObject[] Scenarios;
    [SerializeField] private GameObject[] RewardTowers;
    [SerializeField] private Sprite onImage;
    [SerializeField] private Sprite offImage;
    [SerializeField] private Sprite rewardGreenImage;
    [SerializeField] private Sprite rewardRedImage;

    [Header("---    Animation References    ---")]
    [SerializeField] private Animator sceneTransitionAnimator;
    [SerializeField] private float sceneTransitionTime;

    private Animator rewardAnimator; //Programaticlly get this one as we need to get the relevant on
    
    //Singletons
    private AudioManager audioManager;


    private void Awake() {
        UpdateLevelButtons();
        audioManager = GameObject.FindWithTag("Audio").GetComponent<AudioManager>();
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
                StartCoroutine(AnimateReward(completedLevels-1));
            }
        }
    }

    IEnumerator AnimateReward(int buttonIndex){
        rewardAnimator = Scenarios[buttonIndex].GetComponent<Animator>();
        rewardAnimator.enabled = true; // Enable the Animator component

        yield return new WaitForSeconds(sceneTransitionTime);
        rewardAnimator.SetTrigger("Test"); 
    }

    // TEMP function to reset player progress for Playtesting
    public void ResetProgress() {
        // Resetting level progression
        PlayerPrefs.SetInt("UnlockedLevels", 1);
        PlayerPrefs.SetInt("CompletedLevels", 0);
        // Set the intro story flag to true so the the first time story animation palys again
        PlayerPrefs.SetInt("FirstLaunch", 1);

        PlayerPrefs.Save();
        UpdateLevelButtons();
        SceneManager.LoadScene("MainMenu");

    }

    /// ----------------------------------------------------- ///
    ///         Scene and Scene transition Management         /// 
    /// ----------------------------------------------------- ///

    public void BackToLevelSelection(){
        SceneManager.LoadScene("LvlSelection");
    }

    public void BackToMainMenu(){
        SceneManager.LoadScene("MainMenu");
    }

    // Transition from Breaker Box to Map, with animation
    public void BackToMap(){
        StartCoroutine(GoToMapAnimation());
    }

    IEnumerator GoToMapAnimation(){
        audioManager.playButtonClickSFX();
        sceneTransitionAnimator.SetTrigger("Start");

        //Wait for animation to play
        yield return new WaitForSeconds(sceneTransitionTime);

        //Then Load the right scenario
        SceneManager.LoadScene("Map");
    }

    // Transition from Breaker Box to Scenario, with animation
    public void OpenScenario(int scenarioId){
        audioManager.playButtonClickSFX();
        StartCoroutine(GoToScenarioAnimation(scenarioId));
    }

    IEnumerator GoToScenarioAnimation(int scenarioId){
        // Play the Transition
        audioManager.playButtonClickSFX();
        sceneTransitionAnimator.SetTrigger("Start");

        //Wait for animation to play
        yield return new WaitForSeconds(sceneTransitionTime);

        //Then Load the right scenario
        string sceneName = "Scenario"+scenarioId;
        SceneManager.LoadScene(sceneName);
    }
}
