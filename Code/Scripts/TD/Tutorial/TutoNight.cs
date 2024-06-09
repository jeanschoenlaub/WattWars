using UnityEngine;
using UnityEngine.UI; // Required for UI elements
using TMPro;
using System.Collections;

// Tutorial to show the player how to place a fuel and a diesel generator tower 
public class TutoNight : MonoBehaviour
{
    [Header("Animators References")]
    [SerializeField] public Animator FridgeDialogAnimator; // To make mouse move up and down to direct user
    [SerializeField] public GameObject FFbutton; // To make mouse move up and down to direct user
    [SerializeField] public GameObject TutoTextBox;
    [SerializeField] public Button DialogFridgeClickDetector;
    [SerializeField] TextMeshProUGUI TutoText; 
    

    public float delayFridgeAppear = 25f;
    
    // Internal variables
    private bool firstTimechecker = true ; // flag to enter continuous check only once]    
    public bool isTutorialActive = false;
    private TutorialManager tutoManager;

    private void Awake()
    {
        // Get the TutoPlaceTower component attached to the same GameObject
        tutoManager = GetComponent<TutorialManager>();
    }

    private void Start(){
        TutoTextBox.SetActive(false);
        StartCoroutine(ShowFFButton());
    }

    void Update()
    {
        if (isTutorialActive && WeatherManager.main.isNightFalling && firstTimechecker)
        {
            TutoTextBox.SetActive(true);
            TutoText.text = "Night is falling ...";

            firstTimechecker = false; //So we don't enter this check again
            StartCoroutine(WaitForFridgeEnemyToAppear());
        }
    }

    public void StartTutoNight()
    {
        isTutorialActive = true;
    }

    IEnumerator ShowFFButton(){
        yield return new WaitForSeconds(5f);
        TutoTextBox.SetActive(true);

        TutoText.text = "You can speed up the level";

        yield return new WaitForSeconds(1f);
        FFbutton.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f); //Scale up towerIcone
       
        yield return new WaitForSeconds(3f);
        FFbutton.transform.localScale = new Vector3(1f, 1f, 1f); //Scale up towerIcone
        TutoTextBox.SetActive(false);
        // TutoText.text = "We must feed <color=#85282B>oil</color> 

    }

    IEnumerator WaitForFridgeEnemyToAppear(){
        // Wait for the Fridge enemy to appear and disable fuel tower unto end of tut
        yield return new WaitForSeconds(delayFridgeAppear);
        TutoTextBox.SetActive(false);
        LevelManager.SetGameSpeed(0);
        FridgeDialogAnimator.SetTrigger("PopUp");

        // And after little delay add a listener for the next step
        yield return new WaitForSeconds(0.5f);
        DialogFridgeClickDetector.onClick.AddListener(OnFridgeDialogueClose);
    }

    public void OnFridgeDialogueClose() {
        // Remove the anim and et play again
        FridgeDialogAnimator.SetTrigger("PopDown");
        LevelManager.SetGameSpeed(1);

        StartCoroutine(Task1SelectDieselGen(5f)); // 5 seconds delay to read text
    }

    IEnumerator Task1SelectDieselGen(float delay){

        // Wait before showing text
        yield return new WaitForSeconds(2f);
        TutoText.text = "We can use the diesel generator to produce <color=#F5FF00>electricity</color> at night";
        TutoTextBox.SetActive(true);

        LevelManager.SetGameSpeed(0);

        // Wait for the specified delay
        yield return new WaitForSeconds(5f); 
        TutoTextBox.SetActive(false);
        LevelManager.SetGameSpeed(1);
    }
}