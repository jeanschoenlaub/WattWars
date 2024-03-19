using UnityEngine;
using UnityEngine.UI; // Required for UI elements
using UnityEngine.Events; // For UnityEvent


// Tutorial to show the player how to place a tower
public class TutoPlaceTower : MonoBehaviour
{
    [Header("Tutorial References")]
    [SerializeField] public Button TowerSelectButton; 
    [SerializeField] public UnityEvent onTowerSelectButtonClicked; // Assign a callback for what happens next
    [SerializeField] public GameObject towerPlacementPlotArrow; // Arrow UI poiting to build tower plot
    [SerializeField] public GameObject towerPlacementCircleUI; // Arrow UI poiting to build tower plot
    [SerializeField] private Plot[] plots; // plots availaible for tutorial [0] is the 

    public bool isTutorialActive = false;
    public bool isTowerButtonClicked = false;

    public void StartTutoPlaceTower()
    {
        isTutorialActive = true;
        HighlightButton();
        TowerSelectButton.onClick.AddListener(OnTowerSelectButtonClicked);

        towerPlacementPlotArrow.SetActive(false);
        towerPlacementCircleUI.SetActive(false);
    }

    void Update()
    {
        // We check if the tower was actually built by checking if the first plot 
        // went from constructable to not (because if tower placed --> not constructable)
        if (isTutorialActive & plots[0].constructable == false)
        {
            // Hide the UI elements
            towerPlacementPlotArrow.SetActive(false);
            towerPlacementCircleUI.SetActive(false);
        }
    }


    void HighlightButton()
    {
        TowerSelectButton.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        Color newColor;
        ColorUtility.TryParseHtmlString("#00FF21", out newColor);
        TowerSelectButton.GetComponent<Image>().color = newColor;
        
    }

    public void OnTowerSelectButtonClicked()
    {
        // Reset scale or remove highlight effect
        TowerSelectButton.transform.localScale = Vector3.one;
        TowerSelectButton.onClick.RemoveListener(OnTowerSelectButtonClicked);
        Color newColor;
        ColorUtility.TryParseHtmlString("#E0E0E0", out newColor);
        TowerSelectButton.GetComponent<Image>().color = newColor;

        plots[0].constructable = true;

        // Show both the Arrow UI and Circle UI 
        towerPlacementPlotArrow.SetActive(true);
        towerPlacementCircleUI.SetActive(true);
    }
}
