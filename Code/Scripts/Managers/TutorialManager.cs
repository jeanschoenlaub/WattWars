using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }
    private TutoPlaceTower tutoPlaceTower;
    

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: only if you want it to persist across scenes.
        }
        else
        {
            Destroy(gameObject);
        }
        // Get the TutoPlaceTower component attached to the same GameObject
        tutoPlaceTower = GetComponent<TutoPlaceTower>();
    }

    private void Start()
    {
        // Start the tower placement tutorial
        StartTutoPlaceTower();
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
