using UnityEngine;
using UnityEngine.SceneManagement;

public class Scenario1Reward : MonoBehaviour
{
    public GameObject objectToActivate;
    private AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindWithTag("Audio").GetComponent<AudioManager>();
    }

    public void PlayBreakerSound(){
        audioManager.PlaySFX(audioManager.breakerSwitch);
    } 

    public void PlayBreakerElectricSound(){
        audioManager.PlaySFX(audioManager.electricSound);
    } 

    public void PlayTowerRewardSound(){
        audioManager.PlaySFX(audioManager.rewardSound);
    } 
    
     // This function will be called by the animation event
    public void ActivateLvlRewardUI()
    {
        objectToActivate.SetActive(true);
    }

    public void DeActivateLvlRewardUI()
    {
        objectToActivate.SetActive(false);
        SceneManager.LoadScene("Map"); // To animate the new tower on the map
    }
}
