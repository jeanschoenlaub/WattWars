using UnityEngine;
using UnityEngine.UIElements;

public class BuildingTower : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform energyReceivedBar;
    [SerializeField] public Building buildingConfig;

    [SerializeField] private GameObject switchOnOff;
    [SerializeField] private Animator SwitchButtonAnim;
    
    //Counter which when is equal energyRequired generates money
    private float totalEnergyReceived;
    public bool isSwitchedOn;

    private AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindWithTag("Audio").GetComponent<AudioManager>();
        isSwitchedOn = false;

        Vector3 scale = energyReceivedBar.localScale;
        scale.x = 0;
        energyReceivedBar.localScale= scale;
    }

    public void ReceiveEnergy(float energyReceived)
    {
        
        totalEnergyReceived += energyReceived;
        if (totalEnergyReceived >= buildingConfig.energyRequired){
                totalEnergyReceived = 0;
                LevelManager.main.IncreaseCurrency(buildingConfig.moneyGenerated);
                audioManager.PlaySFX(audioManager.coinsSFX);
            }
        UpdateHUD();
    }

    void UpdateHUD()
    {
        Vector3 scale = energyReceivedBar.localScale;
        if (totalEnergyReceived == 0){scale.x = 0;}
        else { scale.x = (float) totalEnergyReceived / buildingConfig.energyRequired ;}
        energyReceivedBar.localScale = scale;
    
    }
    
    private void OnMouseDown()
    {
        if (switchOnOff != null )
        {
            isSwitchedOn = !isSwitchedOn; // Toggle the state
            SwitchButtonAnim.SetBool("TurnSwitchOn", isSwitchedOn);
        }
    }
}
