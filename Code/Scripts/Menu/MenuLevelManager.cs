using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuLevelManager : MonoBehaviour
{
    public Button[] Level1SelectionButtons;
    public Sprite onImage;
    public Sprite offImage;

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
        int unlockLevelAnimation = PlayerPrefs.GetInt("UnlockedLevelAnimation", 0); //1 if animation to display 0 otherwise

        for (int i = 0; i < Level1SelectionButtons.Length; i++) {
            Animator buttonAnimator = Level1SelectionButtons[i].GetComponent<Animator>();

            if (buttonAnimator != null)
            {
                // If the levels are complete we put breaker switch UI ON
                if (i < completedLevels) {
                    Debug.Log("boolOn");
                    buttonAnimator.SetBool("isBreakerOn",true);
                    Level1SelectionButtons[i].interactable = true;
                    // And if we want to trigger an unlocing animationß
                    // if (unlockLevelAnimation == 1 && i == completedLevels-1) {
                    //     Debug.Log("anim");
                    //     // Set the trigger to start the animation
                    //     buttonAnimator.SetTrigger("StartBreakerAnimation");
                    //     PlayerPrefs.SetInt("UnlockedLevelAnimation", 0); // Reset the animation  flagß
                    // }
                }
                // Else we show the buttons as off
                else{
                    buttonAnimator.SetBool("isBreakerOn",false);
                    Level1SelectionButtons[i].interactable = false;

                    //And make interactable only the next level
                    if (i == completedLevels){
                        Level1SelectionButtons[i].interactable = true;
                    }
                }       
            }
            //If no animator
            else
            {
                Debug.LogWarning("Animator component not found on button: " + Level1SelectionButtons[i].name);
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
}

