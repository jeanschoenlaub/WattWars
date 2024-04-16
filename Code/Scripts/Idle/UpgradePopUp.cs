using UnityEngine;

public class UpgradePopUp : MonoBehaviour
{
    [Header("--------- References ---------")]
    [SerializeField] private GameObject popUpGameObject;
    private bool isUpgradeOpen = false;

    private void Start() {
        popUpGameObject.SetActive(false);
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
