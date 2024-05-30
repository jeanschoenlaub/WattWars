using UnityEngine;

public class ShopPopUp : MonoBehaviour
{
    [Header("--------- References ---------")]
    [SerializeField] private GameObject popUpGameObject;
    private bool isShopOpen = false;

    private void Start() {
        popUpGameObject.SetActive(false);
    }

    public void TogglePopUp(){
        isShopOpen = !isShopOpen;
        
        if (isShopOpen)
        {
            popUpGameObject.SetActive(true);
        }
        else
        {
            popUpGameObject.SetActive(false);
        }
    }
}
