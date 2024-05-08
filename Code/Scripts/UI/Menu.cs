using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [Header("--------- Score and UI ---------")]
    public bool isInTDMode = true;
    [SerializeField] TextMeshProUGUI currencyUI;
    [SerializeField] TextMeshProUGUI livesUI;
    [SerializeField] TextMeshProUGUI waveUI;
    [SerializeField] TextMeshProUGUI waveAnim;
    
    [Header("--------- GameSpeed and Menu ---------")]
    [SerializeField] Button menuButton;
    [SerializeField] private GameObject menuGameObject;
    [SerializeField] TextMeshProUGUI gameSpeedText;
    private bool isMenuOpen = false;

    [Header("--------- GameSound ---------")]
    [SerializeField] Button soundButton;
    [SerializeField] GameObject volumeSlider;
    [SerializeField] Button sfxButton;
    [SerializeField] GameObject sfxSlider;
    [SerializeField] Sprite soundOnSprite;
    [SerializeField] Sprite soundOffSprite;

    private WaveManager waveManager;
    private AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindWithTag("Audio").GetComponent<AudioManager>();
    }   

    void OnApplicationQuit()
    {
        PlayerPrefs.Save(); // Force save PlayerPrefs on exit
    }

    void Start()
    {
        if (isInTDMode){
            waveManager = FindObjectOfType<WaveManager>();
            menuButton.onClick.AddListener(ToggleMenu);
        }
        Debug.Log("ßstatting");
        menuGameObject.SetActive(false);

        
       
        Slider volumeSliderComponent = volumeSlider.GetComponent<Slider>();
        Slider sfxSliderComponent = sfxSlider.GetComponent<Slider>();

        // Setup volume and SFX sliders && Load the saved volume
        SetupVolumeControl(volumeSliderComponent, "SavedVolume", soundButton.image, soundOnSprite, soundOffSprite, volume => audioManager.SetMusicVolume(volume));
        SetupVolumeControl(sfxSliderComponent, "SavedSFXVolume", sfxButton.image, soundOnSprite, soundOffSprite, volume => audioManager.SetSFXVolume(volume));
   
        soundButton.onClick.AddListener(() => ToggleAudio(volumeSliderComponent, "LastVolume", "SavedVolume", audioManager.SetMusicVolume, soundButton.image));
        sfxButton.onClick.AddListener(() => ToggleAudio(sfxSliderComponent, "LastSFXVolume", "SavedSFXVolume", audioManager.SetSFXVolume, sfxButton.image));

    }

   void ToggleAudio(Slider sliderComponent, string lastVolumeKey, string savedVolumeKey, Action<float> setVolume, Image soundButtonImage)
    {
        float currentVolume = sliderComponent.value;
        // If volume is not 0, mute the sound
        if (currentVolume > 0)
        {
            // Remember current slider volume to restore it later
            PlayerPrefs.SetFloat(lastVolumeKey, currentVolume);
            PlayerPrefs.SetFloat(savedVolumeKey, 0);

            // Mute the sound
            sliderComponent.value = 0;
            soundButtonImage.sprite = soundOffSprite;
            setVolume(0);
        }
        else
        {
            // Restore the previous volume, or set it to a default value if not available
            float lastVolume = PlayerPrefs.GetFloat(lastVolumeKey, 0.5f); // Default to 0.5 if no last volume saved
            PlayerPrefs.SetFloat(savedVolumeKey, lastVolume);

            // Unmute the sound to the last volume or default
            sliderComponent.value = lastVolume;
            soundButtonImage.sprite = soundOnSprite;
            setVolume(lastVolume);
        }
    }

    void SetupVolumeControl(Slider sliderComponent, string playerPrefsKey, Image soundButtonImage, Sprite soundOnSprite, Sprite soundOffSprite, Action<float> setVolumeFunc)
    {
        sliderComponent.onValueChanged.AddListener((value) =>
        {
            setVolumeFunc(value);
            PlayerPrefs.SetFloat(playerPrefsKey, value); // Save volume immediately when changed
            PlayerPrefs.Save();

            // Update the sound button icon based on volume
            if (value > 0){ soundButtonImage.sprite = soundOnSprite; }
            else{ soundButtonImage.sprite = soundOffSprite; }
        });

        // Load and set initial slider value from PlayerPrefs or use a default value
        float savedVolume = PlayerPrefs.GetFloat(playerPrefsKey, 0.5f); // Default to 0.5 if not saved
        sliderComponent.value = savedVolume;
        
        // Explicitly apply settings at start to ensure correct initialization
        setVolumeFunc(savedVolume);
        soundButtonImage.sprite = savedVolume > 0 ? soundOnSprite : soundOffSprite;
    }


    public void ToggleMenu(){
        isMenuOpen = !isMenuOpen;
        Debug.Log("menu toggle");
        if (isMenuOpen)
        {
            // If opening the menu, just make it visible and interactable
            Debug.Log("menu open");
            menuGameObject.SetActive(true);
            if (isInTDMode){
                LevelManager.SaveGameSpeed();
            }
        }
        else
        {
            // If closing the menu, first make all buttons non-interactable,
            // then make the menu itself inactive
            menuGameObject.SetActive(false);
            if (isInTDMode){
                LevelManager.ResumeGame(); // Uses the save game speed to resume
            }
        }
    }

    //TO-DO update from onGUI to the Update() function
    private void OnGUI(){
        if (isInTDMode){
            currencyUI.text = LevelManager.main.coins.ToString();
            livesUI.text = LevelManager.main.GetNumeberOfLives().ToString();

            if (waveAnim && waveUI){
                //For the following waves starts at 0 and we want to start with 1 for UI 
            waveAnim.text = "Day "+ (waveManager.currentDayIndex + 1).ToString() + " - Wave " + (waveManager.currentWaveIndex + 1).ToString();
            waveUI.text = "Wave " + (waveManager.currentWaveIndex + 1).ToString() + "/" 
                            +  waveManager.currentDay.waves.Count.ToString();

            }else{
                Debug.Log("UI Text Missing");
            }
        }
    }

    public void BackToMenu(){
        SceneManager.LoadScene("Menu");
    }

    public void BackToIdle(){
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame(){
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    public void Restart(){
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void ToggleGameSpeed(){
        int currentSpeed = LevelManager.GetGameSpeed();
        if (currentSpeed == 1){
            LevelManager.SetGameSpeed(2);
            gameSpeedText.text = "x2";
        } else if (currentSpeed == 2){
            LevelManager.SetGameSpeed(1);
            gameSpeedText.text = "x1";
        }
    }
}
