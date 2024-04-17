using UnityEngine;

public class UpgradePopUp : MonoBehaviour
{
    [Header("--------- References ---------")]
    [SerializeField] private GameObject popUpGameObject;

    // We will dynamically attach the upgrade pop up to the build site clicked
    private BuildSiteManager currentBuildingManager;

    private bool isUpgradeOpen = false;

    private void Start() {
        popUpGameObject.SetActive(false);
    }

    public void SetCurrentBuildingManager(BuildSiteManager manager) {
        currentBuildingManager = manager;
    }

    public void ToggleBuildingOnOff() {
        if (currentBuildingManager != null) {
            currentBuildingManager.ToggleOnOff();
        }else {
            Debug.Log("Bug linking build site and pop up");
        }
    }

    public void TogglePopUp(){
        isUpgradeOpen = !isUpgradeOpen;
        
        if (isUpgradeOpen)
        {
            popUpGameObject.SetActive(true);
        }
        else
        {
            popUpGameObject.SetActive(false);
        }
    }
}
