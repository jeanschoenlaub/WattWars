using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

// Tutorial to show the player how to place a fuel and a diesel generator tower 
public class TutoBuilding : MonoBehaviour
{
    [Header("Animators References")]
    [SerializeField] public Animator MouseAnimator; // To make mouse move up and down to direct user
    
    [Header("Text References")]
    [SerializeField] public GameObject TutoTextBox;
    [SerializeField] TextMeshProUGUI TutoText; 

    [Header("Other References")]
    [SerializeField] private Plot[] plots; // to check construction done
    [SerializeField] public Button[] shopButtonsToDeactivate;

    public bool isTutorialActive = false;
    private bool firstTimechecker = true ;
     private bool secondTimechecker = true ;

    private TutorialManager tutoManager;

    private void Awake()
    {
        // Get the TutoPlaceTower component attached to the same GameObject
        tutoManager = GetComponent<TutorialManager>();
    }

    void Update()
    {
        if (isTutorialActive==true){
            int notConstructableCount = 0;

            foreach (var plot in plots)
            {
                if (!plot.constructable) // If the plot is not constructable
                {
                    notConstructableCount++; // Increment the counter
                }
            }

            foreach (Button btn in shopButtonsToDeactivate)
            {
                btn.interactable = false;
            }
            
            // We check if the first tower was actually built by checking if 1 plot is taken
            // went from constructable to not (because if tower placed --> not constructable)
            if (isTutorialActive && notConstructableCount == 4 && firstTimechecker)
            {
                MouseAnimator.SetTrigger("Hide");
                LevelManager.SetGameSpeed(1);
                StartCoroutine(BuildSolarPanel());
                firstTimechecker = false; //So we don't enter this check again
            }

            if (isTutorialActive && notConstructableCount == 5 && secondTimechecker)
            {
                MouseAnimator.SetTrigger("Hide");
                LevelManager.SetGameSpeed(1);
                secondTimechecker = false; //So we don't enter this check again
                isTutorialActive = false;
            }
        }
    }

    IEnumerator BuildingAnimation(){ 
        yield return new WaitForSeconds(4f);

        TutoTextBox.SetActive(true);
        TutoText.text = "Let's build a data center to generate money from electricity";

        yield return new WaitForSeconds(5f);

        TutoTextBox.SetActive(false);
        LevelManager.SetGameSpeed(0);
        LevelManager.main.IncreaseCurrency(200);
        MouseAnimator.SetTrigger("BuildingAnim");
    }

    IEnumerator BuildSolarPanel(){ 
        yield return new WaitForSeconds(3f);

        TutoTextBox.SetActive(true);
        TutoText.text = "Now, let's Build a solar panel next to the data center";

        yield return new WaitForSeconds(4f);

        TutoTextBox.SetActive(false);
        MouseAnimator.SetTrigger("S4SolarAnim");

        LevelManager.SetGameSpeed(0);
        LevelManager.main.IncreaseCurrency(50);
    }

    private void Start(){
    }

    public void StartTutoBuilding()
    {
        TutoTextBox.SetActive(false);
        
        StartCoroutine(BuildingAnimation());

        isTutorialActive = true;
    }
}
