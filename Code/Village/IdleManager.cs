using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class IdleManager : MonoBehaviour
{
    // Singletons
    private AudioManager audioManager;
    public static IdleManager main;

    [Header("Attributes")]
    [SerializeField] private Sprite[] Maps;
    [SerializeField] private GameObject MapBackground;

    [Header("Animations Management")]
    [SerializeField] private Animator SceneTransitionAnimator;
    [SerializeField] private float sceneTransitionTime = 1f;
    [SerializeField] private Transform[] buildingPositions;
    [SerializeField] private GameObject BuildingAnimGO;
    [SerializeField] private GameObject UnlockAnimGO;

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
        audioManager = GameObject.FindWithTag("Audio").GetComponent<AudioManager>();
    }

    private void Start(){
        LoadMap();
        LoadQuests();
    }

    //Based on what the player as unlocked, load a different map
    public void LoadQuests()
    {
        int questProgress = PlayerPrefs.GetInt("QuestProgress", 0); // Get the quest progress
        if (questProgress == 0){
            QuestScientist1.SetActive(true);
        }
        if (questProgress == 1){
            QuestScientist2.SetActive(true);
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
        StartCoroutine(GoToLevelSelectionAnimation());
    }

    IEnumerator GoToLevelSelectionAnimation(){
        // Play the Transition
        audioManager.playButtonClickSFX();
        SceneTransitionAnimator.SetTrigger("Start");

        //Wait for animation to play
        yield return new WaitForSeconds(sceneTransitionTime);

        //Then Load the Level selection BreakerBox
        SceneManager.LoadScene("LvlSelection");
    }
}
