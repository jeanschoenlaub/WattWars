using UnityEngine;
using UnityEngine.UI; // Required for UI elements
using TMPro;
using System.Collections;

// Tutorial to show the player how to place a fuel and a diesel generator tower 
public class TutoPlaceTower : MonoBehaviour
{
    [Header("Animators References")]
    [SerializeField] public Animator MouseAnimator; // To make mouse move up and down to direct user
    [SerializeField] public Animator FridgeDialogAnimator; // To make mouse move up and down to direct user
    [SerializeField] public Animator CloudsTopAnimator; 
    [SerializeField] public Animator CloudsBottomAnimator; 
    [SerializeField] public GameObject BoxTowerPlacementGO;

    [Header("Text References")]
    [SerializeField] public GameObject TutoTextBox;
    [SerializeField] public Button DialogFridgeClickDetector;
    [SerializeField] TextMeshProUGUI TutoText; 

    [Header("Tower Button References")]
    [SerializeField] public Button TowerSelectButton; 
    [SerializeField] public Button DieselTowerSelectButton; 
    
    [Header("Plot References")]
    [SerializeField] private Plot[] plotsTop; 
    [SerializeField] private Plot[] plotsBottom; 

    [Header("Parameters")]
    [SerializeField] private float firstDelayScooterAppear = 12f;
    [SerializeField] private float secondDelayFridgeAppear = 25f;
    [SerializeField] private int moneyAmountFirstBreak = 50;
    [SerializeField] private int moneyAmountSecondBreak = 100;


    public bool isTutorialActive = false;

    // Internal variables
    private bool firstTimechecker = true ; // flag to enter continuous check only once
    private bool secondTimechecker = true ; // flag to enter continuous check only once
    private bool forcePause = false; // flag to force the gamespeed to 0 until the player has don certain actions 
    
    // Singletons
    private TutorialManager tutoManager;
    private AudioManager audioManager;

    private void Awake()
    {
        // Get the TutoPlaceTower component attached to the same GameObject
        tutoManager = GetComponent<TutorialManager>();
        audioManager = GameObject.FindWithTag("Audio").GetComponent<AudioManager>();
    }

    private void Start(){
        TutoTextBox.SetActive(false);
    }

    void Update()
    {
        //TODO - Not optimal for performance to continuously check 
        int notConstructableTopCount = 0;
        int notConstructableBottomCount = 0;
        
        foreach (var plot in plotsTop)
        {
            if (!plot.constructable) // If the plot is not constructable
            {
                notConstructableTopCount++; // Increment the counter
            }
        }

        foreach (var plot in plotsBottom)
        {
            if (!plot.constructable) // If the plot is not constructable
            {
                notConstructableBottomCount++; // Increment the counter
            }
        }

        if (forcePause){
            LevelManager.PauseGame();
        }
        
        // We check if the first tower was actually built by checking if 1 plot is taken
        // went from constructable to not (because if tower placed --> not constructable)
        if (isTutorialActive && notConstructableTopCount == 1 && firstTimechecker)
        {
            forcePause = false;
            LevelManager.ResumeGame();
            BoxTowerPlacementGO.SetActive(false);

            TutoTextBox.SetActive(false);
            MouseAnimator.SetBool("Hide",true);
            TowerSelectButton.transform.localScale = new Vector3(1f, 1f, 1f); //Scale up towerIcone

            firstTimechecker = false; //So we don't enter this check again
            StartCoroutine(WaitForFridgeEnemyToAppear(secondDelayFridgeAppear)); // 4 seconds delay for electric enemies to appear        
        }

        // We check if the diesel gen tower was actually built by checking if the first plot 
        // went from constructable to not (because if tower placed --> not constructable)
        if (isTutorialActive && notConstructableBottomCount >= 2 && notConstructableBottomCount != 6 && secondTimechecker)
        {
            secondTimechecker = false;

            forcePause = false;
            LevelManager.ResumeGame();

            TowerSelectButton.interactable = true;
            MouseAnimator.SetBool("Hide",true);
            DieselTowerSelectButton.transform.localScale = new Vector3(1f, 1f, 1f); //Scale up towerIcone
            TutoTextBox.SetActive(false);

            StartCoroutine(Task5ToggleGen());
        }
    }

    public void StartTutoPlaceTower()
    {
        isTutorialActive = true;

        TowerSelectButton.interactable = false;
        DieselTowerSelectButton.interactable = false;

        // Start a coroutine to handle the delay
        StartCoroutine(WaitForEnemyToAppear(firstDelayScooterAppear)); // delay for first enemies to appear        
    }

    public void MakeBottomPlotsConstructible()
    {
        foreach (var plot in plotsBottom)
        {
            plot.constructable = true;
        }
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

    // TO-DO Change this to a tuto on dragging
    IEnumerator Task1SelectTower(float delay){
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        LevelManager.main.IncreaseCurrency(moneyAmountFirstBreak);

        TutoText.text = "Let's place a fuel tower to produce <color=#85282B>oil</color>";
        TutoTextBox.SetActive(true);

        forcePause = true;
        LevelManager.PauseGame();

        // UI to indicate the button to click
        TowerSelectButton.interactable = true;
        TowerSelectButton.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f); //Scale up towerIcone
        MouseAnimator.SetTrigger("AnimTower");

        // UI to indicate where to place tower
        CloudsTopAnimator.SetTrigger("CloudFadeOut");
        BoxTowerPlacementGO.SetActive(true);    
    }

    IEnumerator WaitForFridgeEnemyToAppear(float delay){
        // Wait for the Fridge enemy to appear and disable fuel tower unto end of tut
        TowerSelectButton.interactable = false;
        yield return new WaitForSeconds(delay);
        MouseAnimator.SetBool("Hide",false);


        //Then display Fridge text
        LevelManager.PauseGame();

        audioManager.PlaySFX(audioManager.dialogSFX);
        FridgeDialogAnimator.SetTrigger("PopUp");

        // And after little delay add a listener for the next step
        yield return new WaitForSeconds(0.5f);
        DialogFridgeClickDetector.onClick.AddListener(Task3BuildGen);

    }


    public void Task3BuildGen() {
        // Remove the anim and et play again
        FridgeDialogAnimator.SetTrigger("PopDown");
        audioManager.PlaySFX(audioManager.dialogSFX);
        
        LevelManager.ResumeGame();

        // Start a coroutine to handle the delay
        StartCoroutine(Task3SelectDieselGen(5f)); // 5 seconds delay to read text
    }

    // TO-DO Change this to a tuto on draggings
    IEnumerator Task3SelectDieselGen(float delay){

        // Wait before showing text
        yield return new WaitForSeconds(1f);
        TutoText.text = "We can use a diesel generator to convert <color=#85282B>oil</color> to <color=#F5FF00>electricity</color>";
        TutoTextBox.SetActive(true);

        // Wait for the specified delay
        yield return new WaitForSeconds(delay);
        CloudsBottomAnimator.SetTrigger("CloudFadeOut");
        yield return new WaitForSeconds(1f);

        MakeBottomPlotsConstructible();
        LevelManager.main.IncreaseCurrency(moneyAmountSecondBreak);

        TutoText.text = "Place a diesel generator next to the fuel tower";
        
        TutoTextBox.SetActive(true);

        forcePause = true;
        LevelManager.PauseGame();

        // UI to indicate the button to click
        DieselTowerSelectButton.interactable = true;
        DieselTowerSelectButton.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f); //Scale up towerIcone
        MouseAnimator.SetTrigger("AnimDiesel");
    }


    IEnumerator Task5ToggleGen(){
        LevelManager.ResumeGame();


        yield return new WaitForSeconds(1f);
        MouseAnimator.SetBool("Hide",false);

        TutoText.text = "Tap the generator to turn it <color=#32C142>ON </color>";
        TutoTextBox.SetActive(true);

        yield return new WaitForSeconds(5f);

        TowerSelectButton.interactable = false;
        TutoTextBox.SetActive(false);
    }
}
