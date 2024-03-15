using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuLevelManager : MonoBehaviour
{
    public Button[] Level1SelectionButtons;

    private void Awake() {
        Debug.Log("Current unlocked sc: " + PlayerPrefs.GetInt("UnlockedLevels"));
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevels",1);
        for (int i=0; i<Level1SelectionButtons.Length; i++){
            Level1SelectionButtons[i].interactable = false;
        }
        for (int i = 0; i < unlockedLevel; i++){
            Level1SelectionButtons[i].interactable = true;
        }
    }
    
    public void OpenScenario(int scenarioId){
        string sceneName = "Scenario"+scenarioId;
        SceneManager.LoadScene(sceneName);
    }
}
