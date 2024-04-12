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

    private AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindWithTag("Audio").GetComponent<AudioManager>();
    }

    public void ReceiveEnergy(int elecAmount, int fuelAmount)
    {
        
        if (elecAmount != 0) {
            elecReceived += elecAmount;
            if (elecReceived == elecRequired){
                elecReceived = 0;
                LevelManager.main.IncreaseCurrency(moneyGenerated);
                audioManager.PlaySFX(audioManager.coinsSFX);
            }
            UpdateHUD();
        }
        if (fuelAmount != 0) {
            Debug.Log("receiving fuel");
            fuelReceived += fuelAmount;
            if (fuelReceived == fuelRequired){
                fuelReceived = 0;
                LevelManager.main.IncreaseCurrency(moneyGenerated);
                audioManager.PlaySFX(audioManager.coinsSFX);
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
