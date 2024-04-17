using UnityEngine;

public class BuildPopUp : MonoBehaviour
{
    [Header("--------- References ---------")]
    [SerializeField] private GameObject popUpGameObject;

    // We will dynamically attach the upgrade pop up to the build site clicked
    private BuildSiteManager currentBuildingManager;

    private bool isBuildPopUpOpen = false;

    private void Start() {
        popUpGameObject.SetActive(false);
    }

    public void TogglePopUp(){
        isBuildPopUpOpen = !isBuildPopUpOpen;
        
        if (isBuildPopUpOpen)
        {
            popUpGameObject.SetActive(true);
        }
        else
        {
            popUpGameObject.SetActive(false);
        }
    }

    public void SetCurrentBuildingManager(BuildSiteManager manager) {
        currentBuildingManager = manager;
    }

    public void SetBuilding(IdleBuilding building) {
        if (currentBuildingManager != null) {
            currentBuildingManager.SetBuilding(building);
        }else {
            Debug.Log("Bug linking build site and pop up");
        }
    }

}
