using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class splashScreenManager : MonoBehaviour
{

    [Header("Attiributes")]
    [SerializeField] private Animator StoryAnimator;
    [SerializeField] private GameObject StoryAnimatorGO;

    //Singletons
    private AudioManager audioManager;

    // Flag key for PlayerPrefs
    private const string FirstLaunchKey = "FirstLaunch";

    
    private void Awake()
    {
        // Initialize the AudioManager instance
        audioManager = GameObject.FindWithTag("Audio").GetComponent<AudioManager>();
        StoryAnimatorGO.SetActive(false);

        // Check if it's the first launch
        if (IsFirstLaunch())
        {
            // Play the one-time animation
            StartCoroutine(PlayOneTimeAnimation());
            
            // Set the flag to false after playing the animation
            PlayerPrefs.SetInt(FirstLaunchKey, 0);
            PlayerPrefs.Save();
        }
    }

    // Method to check if it's the first launch
    private bool IsFirstLaunch()
    {
        return PlayerPrefs.GetInt(FirstLaunchKey, 1) == 1;
    }

    // Method to play the one-time animation
    IEnumerator PlayOneTimeAnimation()
    {
        StoryAnimatorGO.SetActive(true);
        StoryAnimator.SetTrigger("StartAnimation");

        yield return new WaitForSeconds(8f);
        StoryAnimatorGO.SetActive(false);
    }

    public void GoToMap(){
        audioManager.playButtonClickSFX();
        SceneManager.LoadScene("Map");
    }
}