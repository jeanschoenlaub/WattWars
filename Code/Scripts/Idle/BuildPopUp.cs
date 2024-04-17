using UnityEngine;

public class BuildPopUp : MonoBehaviour
{
    [Header("--------- References ---------")]
    [SerializeField] private GameObject popUpGameObject;
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
}
