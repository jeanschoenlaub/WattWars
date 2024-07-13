using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class IdleManager : MonoBehaviour
{
    // Singletons
    private AudioManager audioManager;
    public static IdleManager main;
    private SceneTransitionManager sceneTransitonManager;

    [Header("Attributes")]
    [SerializeField] private Sprite[] Maps;
    [SerializeField] private GameObject MapBackground;

    [Header("Animations Management")]
    [SerializeField] private Transform[] buildingPositions;
    [SerializeField] private GameObject BuildingAnimGO;
    [SerializeField] private GameObject UnlockAnimGO;

    [Header("BB Management")]
    [SerializeField] private GameObject BBLocked1;
    [SerializeField] private Button BBLockedButton1;

    [Header("Quest Management")]
    [SerializeField] private GameObject QuestScientist1;
    [SerializeField] private GameObject QuestScientist1StillScientist;
    [SerializeField] private GameObject QuestScientist1HammerScientist;
    [SerializeField] private GameObject QuestScientist2;
    
    private void Awake()
    {
        if (main != null && main != this)
        {
            Destroy(gameObject); // Ensure singleton integrity
            return;
        }
        main = this;

        //Get the Singletons
        audioManager = GameObject.FindWithTag("Audio").GetComponent<AudioManager>();
        sceneTransitonManager = GameObject.FindWithTag("SceneTransition").GetComponent<SceneTransitionManager>();
    }

    private void Start(){
        LoadQuests();
        StartCoroutine(sceneTransitonManager.StartVillageSceneEntryAnimation());
    }

    //Based on what the player as unlocked, load a different map
    public void LoadQuests()
    {
        int questProgress = PlayerPrefs.GetInt("QuestProgress", 0); // Get the quest progress
        if (questProgress == 0){
            QuestScientist1.SetActive(true);
            QuestScientist1StillScientist.SetActive(false);
            BBLocked1.SetActive(true);
            BBLockedButton1.interactable = false;
        }
        if (questProgress == 1){
            QuestScientist2.SetActive(true);
            BBLocked1.SetActive(true);
            BBLockedButton1.interactable = false;
        }
    }

    public void TriggerUnlockAnimation(int locationIndex){
        UnlockAnimGO.SetActive(true);
        Animator unlockAnimator = UnlockAnimGO.GetComponent<Animator>();

        unlockAnimator.SetTrigger("UnlockNextBreaker");
    }

    // Triggered via button logic
    public void ChangeHammerScientistToFacingCam(){
       QuestScientist1StillScientist.SetActive(true);
       QuestScientist1HammerScientist.SetActive(false);
    }

    public void GoToLevelSelection(){
        audioManager.playButtonClickSFX();
        SceneManager.LoadScene("LvlSelection");
    }

    public void GoToMainMenu(){
        StartCoroutine(sceneTransitonManager.StartSceneExitToMainMenuAnimation());
    }    

    // TEMP function to reset player progress for Playtesting
    public void ResetProgress() {
        // Resetting level progression
        PlayerPrefs.SetInt("UnlockedLevels", 1);
        PlayerPrefs.SetInt("CompletedLevels", 0);
        PlayerPrefs.SetInt("QuestProgress", 0);
        PlayerPrefs.SetInt("UnlockedLevelAnimation", 0);
        
        // Set the intro story flag to true so the the first time story animation palys again
        PlayerPrefs.SetInt("FirstLaunch", 1);
        
        // Save and switch to splashCreen
        PlayerPrefs.Save();
        SceneManager.LoadScene("MainMenu");
    }
}
