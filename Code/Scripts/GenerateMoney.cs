using UnityEngine;

public class GenerateMoney : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform elecGenBar; //HUD
    [SerializeField] private RectTransform fuelGenBar; //HUD

    [Header("Attributes")]
    [SerializeField] private int moneyGenerated;
    [SerializeField] private int fuelRequired;
    [SerializeField] private int elecRequired;
    
    //Counter which when is equal energyRequired generates money
    private int elecReceived;
    private int fuelReceived;

    public void ReceiveEnergy(int amount)
    {
        {
            elecReceived += amount;
            if (elecReceived == elecRequired){
                elecReceived = 0;
                LevelManager.main.IncreaseCurrency(moneyGenerated);
            }
            UpdateHUD();
        }
    }

    void UpdateHUD()
    {
        // Update electric lives bar scale
        if (elecGenBar != null) {
            Vector3 scale = elecGenBar.localScale;
            if (elecReceived == 0){scale.x = 0;}
            else { scale.x = (float) elecReceived / elecRequired ;}
            elecGenBar.localScale = scale;
        }
        
        // Update fuel lives bar scale
        if (fuelGenBar != null) {
            Vector3 scale = fuelGenBar.localScale;
            if (fuelReceived == 0){scale.x = 0;}
            else { scale.x = (float) fuelReceived / fuelRequired ;}
            fuelGenBar.localScale = scale;
        }
    }
}
