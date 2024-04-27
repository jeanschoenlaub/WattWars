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
    [SerializeField] public Animator FridgeDialogAnimator; // To make mouse move up and down to direct user

    [Header("Text References")]
    [SerializeField] public GameObject TutoTextBox;
    [SerializeField] public Button DialogFridgeClickDetector;
    [SerializeField] TextMeshProUGUI TutoText; 

    [Header("Tower Button References")]
    [SerializeField] public Button TowerSelectButton; 
    [SerializeField] public UnityEvent onTowerSelectButtonClicked; 
    [SerializeField] public Button DieselTowerSelectButton; 
    [SerializeField] public UnityEvent onDieselTowerSelectButtonClicked; 
    
    [Header("Plot References")]
    [SerializeField] private Plot[] plots; // rest of plots 

    [Header("Parameters")]
    public float firstDelayScooterAppear = 12f;
    public float secondDelayFridgeAppear = 25f;
    public bool isTutorialActive = false;

    // Internal variables
    private bool firstTimechecker = true ; // flag to enter continuous check only once
    private bool secondTimechecker = true ; // flag to enter continuous check only once
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
            StartCoroutine(WaitForFridgeEnemyToAppear(secondDelayFridgeAppear)); // 4 seconds delay for electric enemies to appear        
        }

        // We check if the diesel gen tower was actually built by checking if the first plot 
        // went from constructable to not (because if tower placed --> not constructable)
        if (isTutorialActive && notConstructableCount >= 5 && secondTimechecker)
        {
            secondTimechecker = false;

            TowerSelectButton.interactable = true;
            MouseAnimator.SetTrigger("Hide");
            TutoTextBox.SetActive(false);

            StartCoroutine(Task5ToggleGen());
        }
    }

    public void StartTutoPlaceTower()
    {
        isTutorialActive = true;

        TowerSelectButton.interactable = false;
        DieselTowerSelectButton.interactable = false;

        TutoText.text = "Your in charge of City xxxx";
        TutoTextBox.SetActive(true);

        // Start a coroutine to handle the delay
        StartCoroutine(WaitForEnemyToAppear(firstDelayScooterAppear)); // delay for first enemies to appear        
    }

    IEnumerator WaitForEnemyToAppear(float delay)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        TutoText.text = "We must feed <color=#85282B>oil</color> to that scooter before it reaches the grid !!";
        TutoTextBox.SetActive(true);

        // Start a coroutine to handle the delay
        StartCoroutine(Task1SelectTower(5f)); // 5 seconds delay to read text
    }

    IEnumerator Task1SelectTower(float delay){
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        LevelManager.main.IncreaseCurrency(100);

        TutoText.text = "Let's place a fuel tower to produce <color=#85282B>oil</color>";
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

        TutoText.text = "Place the tower on any grass square";
    }

    IEnumerator WaitForFridgeEnemyToAppear(float delay){
        // Wait for the Fridge enemy to appear and disable fuel tower unto end of tut
        TowerSelectButton.interactable = false;
        yield return new WaitForSeconds(delay);

        //Then display Fridge text
        LevelManager.SetGameSpeed(0);
        FridgeDialogAnimator.SetTrigger("PopUp");

        // And after little delay add a listener for the next step
        yield return new WaitForSeconds(0.5f);
        DialogFridgeClickDetector.onClick.AddListener(Task3BuildGen);

    }


    public void Task3BuildGen() {
        // Remove the anim and et play again
        FridgeDialogAnimator.SetTrigger("PopDown");
        
        LevelManager.SetGameSpeed(1);

        // Start a coroutine to handle the delay
        StartCoroutine(Task3SelectDieselGen(5f)); // 5 seconds delay to read text
    }

    IEnumerator Task3SelectDieselGen(float delay){

        // Wait before showing text
        yield return new WaitForSeconds(1f);
        TutoText.text = "We can use a diesel generator to convert <color=#85282B>oil</color> to <color=#F5FF00>electricity</color>";
        TutoTextBox.SetActive(true);

        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        LevelManager.main.IncreaseCurrency(200);

        TutoText.text = "Select the diesel generator tower";
        TutoTextBox.SetActive(true);

        LevelManager.SetGameSpeed(0);

        // UI to indicate the button to click
        DieselTowerSelectButton.interactable = true;
        DieselTowerSelectButton.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f); //Scale up towerIcone
        MouseAnimator.SetTrigger("AnimDiesel");

        // Trigger for next step
        DieselTowerSelectButton.onClick.AddListener(Task4PlaceDieselGenerator);
    }

    public void Task4PlaceDieselGenerator()
    {
        // Remove UI to indicate the button to click
        MouseAnimator.SetTrigger("Hide");
        DieselTowerSelectButton.transform.localScale = Vector3.one;
        DieselTowerSelectButton.onClick.RemoveListener(Task4PlaceDieselGenerator);

        TutoText.text = "Place the generator next to a fuel tower";
    }

    IEnumerator Task5ToggleGen(){
        LevelManager.SetGameSpeed(1);

        yield return new WaitForSeconds(1f);

        TutoText.text = "Tap the generator to turn it <color=#32C142>ON </color>";
        TutoTextBox.SetActive(true);

        yield return new WaitForSeconds(5f);

        TowerSelectButton.interactable = false;
        TutoTextBox.SetActive(false);
        tutoManager.EndTutorial();
    }
}
