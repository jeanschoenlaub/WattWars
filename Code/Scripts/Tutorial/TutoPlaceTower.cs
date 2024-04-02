using UnityEngine;
using UnityEngine.UI; // Required for UI elements
using UnityEngine.Events; // For UnityEvent
using TMPro;
using System.Collections;

// Tutorial to show the player how to place a fuel and a diesel generator tower 
public class TutoPlaceTower : MonoBehaviour
{
    [Header("Animators References")]
    [SerializeField] public Animator MouseAnimator; // To make mouse move up and down to direct user
    
    [Header("Text References")]
    [SerializeField] public GameObject TutoTextBox;
    [SerializeField] TextMeshProUGUI TutoText; 

    [Header("Tower Button References")]
    [SerializeField] public Button TowerSelectButton; 
    [SerializeField] public UnityEvent onTowerSelectButtonClicked; 
    [SerializeField] public Button DieselTowerSelectButton; 
    [SerializeField] public UnityEvent onDieselTowerSelectButtonClicked; 
    
    [Header("Plot References")]
    [SerializeField] private Plot[] plots; // rest of plots 

    public bool isTutorialActive = false;
    public bool isTowerButtonClicked = false;
    bool firstTimechecker = true ; // flag to enter continuous check only once

    private TutorialManager tutoManager;

    private void Awake()
    {
        // Get the TutoPlaceTower component attached to the same GameObject
        tutoManager = GetComponent<TutorialManager>();
    }

    private void Start(){
        TutoTextBox.SetActive(false);
    }

    void Update()
    {

        //TODO - Not optimal for performance to continuously check 
        int notConstructableCount = 0;

        foreach (var plot in plots)
        {
            if (!plot.constructable) // If the plot is not constructable
            {
                notConstructableCount++; // Increment the counter
            }
        }
        
        // We check if the first tower was actually built by checking if 1 plot is taken
        // went from constructable to not (because if tower placed --> not constructable)
        if (isTutorialActive && notConstructableCount == 1 && firstTimechecker)
        {
            LevelManager.SetGameSpeed(1);

            TutoTextBox.SetActive(false);

            firstTimechecker = false; //So we don't enter this check again
            TowerSelectButton.interactable = false; //To make sure the player doesn't spend all his money
            StartCoroutine(Task3PlaceDieselGenTower(25f)); // 4 seconds delay for electric enemies to appear        
        }

        // We check if the diesel gen tower was actually built by checking if the first plot 
        // went from constructable to not (because if tower placed --> not constructable)
        if (isTutorialActive && notConstructableCount == 5)
        {
            LevelManager.SetGameSpeed(1);
            TowerSelectButton.interactable = true;
            tutoManager.EndTutorial();
            MouseAnimator.SetTrigger("Hide");
            TutoTextBox.SetActive(false);
        }
    }

    public void StartTutoPlaceTower()
    {
        isTutorialActive = true;

        TowerSelectButton.interactable = false;
        DieselTowerSelectButton.interactable = false;
        // Start a coroutine to handle the delay
        StartCoroutine(WaitForEnemyToAppear(5f)); // 1.5 seconds delay for first enemies to appear        
    }

    IEnumerator WaitForEnemyToAppear(float delay)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        TutoText.text = "We must feed that scooter before it reaches the grid !!";
        TutoTextBox.SetActive(true);

        // Start a coroutine to handle the delay
        StartCoroutine(Task1SelectTower(5f)); // 3 seconds delay to read text
    }

    IEnumerator Task1SelectTower(float delay){
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        TutoText.text = "Let's place a fuel tower";
        TutoTextBox.SetActive(true);

        LevelManager.SetGameSpeed(0);

        // UI to indicate the button to click
        TowerSelectButton.interactable = true;
        TowerSelectButton.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f); //Scale up towerIcone
        MouseAnimator.SetTrigger("AnimTower");

        // Trigger for next step
        TowerSelectButton.onClick.AddListener(Task2PlaceTower);
    }

    public void Task2PlaceTower()
    {
        // Remove UI to indicate the button to click
        MouseAnimator.SetTrigger("Hide");
        TowerSelectButton.transform.localScale = Vector3.one;
        TowerSelectButton.onClick.RemoveListener(Task2PlaceTower);

        TutoText.text = "Place Tower anywhere";
    }

    IEnumerator Task3PlaceDieselGenTower(float delay){
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        TutoText.text = "Let's place a diesel generator tower";
        TutoTextBox.SetActive(true);

        DieselTowerSelectButton.interactable = true;

        LevelManager.SetGameSpeed(0);
    }
}
