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
        //LoadMap();
        LoadQuests();
        StartCoroutine(sceneTransitonManager.StartVillageSceneEntryAnimation());
    }

    //Based on what the player as unlocked, load a different map
    public void LoadQuests()
    {
        int questProgress = PlayerPrefs.GetInt("QuestProgress", 0); // Get the quest progress
        if (questProgress == 0){
            QuestScientist1.SetActive(true);
            BBLocked1.SetActive(true);
            BBLockedButton1.interactable = false;
        }
        if (questProgress == 1){
            QuestScientist2.SetActive(true);
            BBLocked1.SetActive(true);
            BBLockedButton1.interactable = false;
        }
    }

    //Based on what the player as unlocked, load a different map
    public void LoadMap()
    {
        // Load the relevant map based on player progress
        int completedLvls = PlayerPrefs.GetInt("CompletedLevels", 0); // Get the number of completed levels
        MapBackground.GetComponent<Image>().sprite = Maps[completedLvls];

        // If the flag UnlockedLevelAnimation is true trigger the BreakerBox Animation
        int BuildingAnimationLoc = PlayerPrefs.GetInt("UnlockedLevelAnimation", 0); 
        if (BuildingAnimationLoc != 0){
            // Each Level's last unlock triggers unlocking the next level 
            if (BuildingAnimationLoc % 4 == 0){
                TriggerUnlockAnimation(BuildingAnimationLoc);
            }
            else{
                TriggerBuildingAnimation(BuildingAnimationLoc);
            }
            PlayerPrefs.SetInt("UnlockedLevelAnimation", 0); // Reset the animation flag 
        }
    }

    public void TriggerUnlockAnimation(int locationIndex){
        UnlockAnimGO.SetActive(true);
        Animator unlockAnimator = UnlockAnimGO.GetComponent<Animator>();

        unlockAnimator.SetTrigger("UnlockNextBreaker");
    }

    public void TriggerBuildingAnimation(int locationIndex){
        BuildingAnimGO.SetActive(true);
        Animator buildingAnimator = BuildingAnimGO.GetComponent<Animator>();

        // Position the BuildingAnimGO at the specified location
        BuildingAnimGO.transform.position = buildingPositions[locationIndex-1].position;

        buildingAnimator.SetTrigger("Building");
    }

    public void GoToLevelSelection(){
        audioManager.playButtonClickSFX();
        SceneManager.LoadScene("LvlSelection");
    }

    public void GoToMainMenu(){
        StartCoroutine(sceneTransitonManager.StartSceneExitToMainMenuAnimation());
    }    
}
