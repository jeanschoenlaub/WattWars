using UnityEngine;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    // If Tuto attached and first time playing level the tutorial is triggered 
    [Header("Tutorial References - Only attach relevant one if any")]
    [SerializeField] public TutoPlaceTower tutoPlaceTower;
    [SerializeField] public TutoNight tutoNight;
    [SerializeField] public TutoPopUp tutoPopUp;
    [SerializeField] GameObject gameSpeedButton;
   
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
            // Start the coroutine to wait for the tutorial pop-up to close
            StartCoroutine(WaitForPopUpToClose());
            // Get the TutoPlaceTower component attached to the same GameObjec
        } 

        // Start The night tutorial - TODO and logic for only first time
        if (tutoNight){
            // Get the TutoPlaceTower component attached to the same GameObject
            tutoNight = GetComponent<TutoNight>();
            tutoNight.StartTutoNight();
        } 
    }

    private IEnumerator WaitForPopUpToClose()
    {
        // Wait 4 seconds for the wave banner to show before showing tutorial pop=up
        yield return new WaitForSeconds(4f); 
        tutoPopUp.ShowPopUp1();

        // Wait until the pop-ups are closed
        while (!tutoPopUp.ArePopUpsClosed())
        {
            yield return null; // Wait for the next frame
        }

        // Proceed with the tower placement tutorial
        tutoPlaceTower.StartTutoPlaceTower();

        // We also disable the game speed button to simplify tutorial logic
        gameSpeedButton.SetActive(false);
    }
}
