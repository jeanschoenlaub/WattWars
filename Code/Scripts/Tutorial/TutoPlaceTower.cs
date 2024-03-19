using UnityEngine;
using UnityEngine.UI; // Required for UI elements
using UnityEngine.Events; // For UnityEvent
using TMPro;
using System.Collections;


// Tutorial to show the player how to place a tower
public class TutoPlaceTower : MonoBehaviour
{
    [Header("Animators References")]
    [SerializeField] public Animator CloudAnimator; // To remove Clouds before allowing to build towers
    [SerializeField] public Animator MouseAnimator; // To make mouse move up and down to direct user
    
    [Header("Text References")]
    [SerializeField] public GameObject TutoTextBox;
    [SerializeField] TextMeshProUGUI TutoText; 

    [Header("Tower Button References")]
    [SerializeField] public Button TowerSelectButton; 
    [SerializeField] public UnityEvent onTowerSelectButtonClicked; // Assign a callback for what happens next
    
    [Header("Plot References")]
    [SerializeField] public GameObject towerPlacementCircleUI; // Arrow UI poiting to build tower plot
    [SerializeField] private Plot[] plots; // plots availaible for tutorial [0] is the 

    public bool isTutorialActive = false;
    public bool isTowerButtonClicked = false;

    private TutorialManager tutoManager;

    private void Awake()
    {
        // Get the TutoPlaceTower component attached to the same GameObject
        tutoManager = GetComponent<TutorialManager>();
    }

    private void Start(){
        TutoTextBox.SetActive(false);
        towerPlacementCircleUI.SetActive(false);
    }

    void Update()
    {
        // We check if the tower was actually built by checking if the first plot 
        // went from constructable to not (because if tower placed --> not constructable)

        if (isTutorialActive && (plots[0].constructable == false))
        {
            //LevelManager.SetGameSpeed(1);
            tutoManager.EndTutorial();
            MouseAnimator.SetTrigger("Hide");
            TutoTextBox.SetActive(false);
            towerPlacementCircleUI.SetActive(false);
        }
    }

    public void StartTutoPlaceTower()
    {
        isTutorialActive = true;
        CloudAnimator.SetTrigger("RemoveClouds");


        // Start a coroutine to handle the delay
        StartCoroutine(WaitForEnemyToAppear(3f)); // 3 seconds delay for first enemies to appear        
    }

    IEnumerator WaitForEnemyToAppear(float delay)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        TutoText.text = "We must feed that scooter before it reaches the grid !!";
        TutoTextBox.SetActive(true);

        //LevelManager.SetGameSpeed(0);

        // Start a coroutine to handle the delay
        StartCoroutine(Task1SelectTower(3f)); // 3 seconds delay for first enemies to appear
    }

    IEnumerator Task1SelectTower(float delay){
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        TutoText.text = "Let's place a fuel tower";
        TutoTextBox.SetActive(true);

        HighlightButton();
        TowerSelectButton.onClick.AddListener(OnTowerSelectButtonClicked);
    }

    

    void HighlightButton()
    {
        TowerSelectButton.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f); //Scale up towerIcone
        // Show both the Arrow UI 
        MouseAnimator.SetTrigger("AnimTower");
    }

    public void OnTowerSelectButtonClicked()
    {
        // Reset scale or remove highlight effect
        TowerSelectButton.transform.localScale = Vector3.one;
        TowerSelectButton.onClick.RemoveListener(OnTowerSelectButtonClicked);

        TutoText.text = "Place Tower";

        plots[0].constructable = true;

        // Show both the Arrow UI and Circle UI 
        MouseAnimator.SetTrigger("AnimMousePlot");
        towerPlacementCircleUI.SetActive(true);
    }
}
