using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IdleManager : MonoBehaviour
{
    public static IdleManager main;

    [Header("Attributes")]
    [SerializeField] public Sprite[] Maps;
    [SerializeField] public GameObject MapBackground;

    // For animating building  
    [SerializeField] public Transform[] buildingPositions;
    [SerializeField] public GameObject BuildingAnimGO;
    [SerializeField] public GameObject UnlockAnimGO;

    // Singletons
    private AudioManager audioManager;
    
    private void Awake()
    {
        if (main != null && main != this)
        {
            Destroy(gameObject); // Ensure singleton integrity
            return;
        }
        main = this;
    }

    private void Start(){
        LoadMap();
    }

    //Based on what the player as unlocked, load a different map
    public void LoadMap()
    {
        int completedLvls = PlayerPrefs.GetInt("CompletedLevels", 0); // Get the number of completed levels
        MapBackground.GetComponent<Image>().sprite = Maps[completedLvls];

        int BuildingAinmationLoc = PlayerPrefs.GetInt("UnlockedLevelAnimation", 0); 

        if (BuildingAinmationLoc != 0){
            // Each Level's last unlock triggers unlocking the next level 
            if (BuildingAinmationLoc % 4 == 0){
                TriggerUnlockAnimation(BuildingAinmationLoc);
            }
            else{
                TriggerBuildingAnimation(BuildingAinmationLoc);
            }
            PlayerPrefs.SetInt("UnlockedLevelAnimation", 0); // Reset the flag 
        }
    }

    public void TriggerUnlockAnimation(int locationIndex){
        Debug.Log("unlock anim2");
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
        SceneManager.LoadScene("LvlSelection");
    }
}
