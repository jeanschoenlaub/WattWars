using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuLevelManager : MonoBehaviour
{
    public Button[] Level1SelectionButtons;

    // private void Awake() {
    //     int unlockedLevel = PlayerPrefs.GetInt
    // }
    
    public void OpenChapter1scenario(int scenarioId){
        string sceneName = "Chapter1Scenario"+scenarioId;
        SceneManager.LoadScene(sceneName);
    }
}
