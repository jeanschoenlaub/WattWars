using UnityEngine;

public class TutoPopUp : MonoBehaviour
{
    [Header("Animators References")]
    [SerializeField] public GameObject popUp1;
    [SerializeField] public GameObject popUp2;

    [SerializeField] public GameObject WaveBannerGameObject;

    private bool popUpsClosed = false;

    private AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindWithTag("Audio").GetComponent<AudioManager>();
    }   

    private void Start(){
        popUp1.SetActive(false);
        popUp2.SetActive(false);
        WaveBannerGameObject.SetActive(false); // We will also disable the first wave baner if tutorial is On
    }

    public void ShowPopUp1(){
        audioManager.playButtonClickSFX();
        LevelManager.SetGameSpeed(0);
        popUp1.SetActive(true);
        popUp2.SetActive(false);
        popUpsClosed = false;
    }

    public void ShowPopUp2(){
        audioManager.playButtonClickSFX();
        popUp1.SetActive(false);
        popUp2.SetActive(true);
        popUpsClosed = false;
    }

    public void ClosePopUps(){
        audioManager.playButtonClickSFX();
        LevelManager.SetGameSpeed(1);
        popUp1.SetActive(false);
        popUp2.SetActive(false);
        popUpsClosed = true;

        WaveBannerGameObject.SetActive(true); // We will re-enable the wave baner (To only show from Second Day)
    }

    public bool ArePopUpsClosed(){
        return popUpsClosed;
    }
}
