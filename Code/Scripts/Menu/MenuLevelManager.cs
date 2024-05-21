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

        for (int i = 0; i < Level1SelectionButtons.Length; i++) {
            if (i < completedLevels) {Level1SelectionButtons[i].GetComponent<Image>().sprite = onImage;}
            else { Level1SelectionButtons[i].GetComponent<Image>().sprite = offImage; }

            // If the levels are locked we disable them
            if (i < unlockedLevel) {Level1SelectionButtons[i].interactable = true;}
            else {Level1SelectionButtons[i].interactable = false;}
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

