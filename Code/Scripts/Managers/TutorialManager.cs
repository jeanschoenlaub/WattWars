using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    private TutoPlaceTower tutoPlaceTower;

    private void Awake()
    {
        // Get the TutoPlaceTower component attached to the same GameObject
        tutoPlaceTower = GetComponent<TutoPlaceTower>();
    }

    private void Start()
    {
        // Optionally pause the game here, if that's part of your game flow
        LevelManager.SetGameSpeed(0); // Assuming LevelManager controls the game speed
        
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
}
