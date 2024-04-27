using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] GameObject GameSpeedButton;
    public static TutorialManager Instance { get; private set; }
    public TutoPlaceTower tutoPlaceTower;
    
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
        // Start the tower placement tutorial
        if (tutoPlaceTower){
            // Get the TutoPlaceTower component attached to the same GameObject
            tutoPlaceTower = GetComponent<TutoPlaceTower>();
            StartTutoPlaceTower();

            // We also disabel the game speed button to simplify tutorial logic
            GameSpeedButton.SetActive(false);
        } 
    }

    public void StartTutoPlaceTower()
    {
        // Check if the TutoPlaceTower component is available and start the tutorial
        if (tutoPlaceTower != null)
        {
            tutoPlaceTower.StartTutoPlaceTower();
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
