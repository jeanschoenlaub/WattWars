using UnityEngine;
using UnityEngine.SceneManagement;

public class splashScreenManager : MonoBehaviour
{
    //Singletons
    private AudioManager audioManager;


    private void Awake() {
        audioManager = GameObject.FindWithTag("Audio").GetComponent<AudioManager>();
    }

    public void GoToMap(){
        audioManager.playButtonClickSFX();
        SceneManager.LoadScene("Map");
    }
}