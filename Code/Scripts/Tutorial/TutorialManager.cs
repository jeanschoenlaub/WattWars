using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    // If Tuto attached and first time playing level the tutorial is triggered 
    [Header("Tutorial References - Only attach relevant one if any")]
    [SerializeField] public TutoPlaceTower tutoPlaceTower;
    [SerializeField] public TutoNight tutoNight;

    [SerializeField] GameObject GameSpeedButton;
    public static TutorialManager Instance { get; private set; }

    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
    }

    private void Start()
    {
        // Start the tower placement tutorial - TODO and logic for only first time
        if (tutoPlaceTower){
            // Get the TutoPlaceTower component attached to the same GameObjec
            tutoPlaceTower = GetComponent<TutoPlaceTower>();
            tutoPlaceTower.StartTutoPlaceTower();

            // We also disable the game speed button to simplify tutorial logic
            GameSpeedButton.SetActive(false);
        } 

        // Start The night tutorial - TODO and logic for only first time
        if (tutoNight){
            // Get the TutoPlaceTower component attached to the same GameObject
            tutoNight = GetComponent<TutoNight>();
            tutoNight.StartTutoNight();

            // We also disabel the game speed button to simplify tutorial logic
            //GameSpeedButton.SetActive(false);
        } 
    }

    public void StartTutoPlaceTower()
    {
        // Check if the TutoPlaceTower component is available and start the tutorial
        if (tutoPlaceTower != null)
        {
            
        }
        else
        {
            Debug.LogError("TutoPlaceTower component is not found on the GameObject.");
        }
    }

    // Method to be called when the tutorial is finished
    public void EndTutorial()
    {
        //LevelManager.SetGameSpeed(1);
    }
}
