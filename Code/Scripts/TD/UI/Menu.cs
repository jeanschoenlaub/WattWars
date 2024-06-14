using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{   
    [Header("--------- GameSpeed and Menu ---------")]
    [SerializeField] Button menuButton;
    [SerializeField] private GameObject menuGameObject;
    
    private bool isMenuOpen = false;

    [Header("--------- GameSound ---------")]
    [SerializeField] Button soundButton;
    [SerializeField] GameObject volumeSlider;
    [SerializeField] Button sfxButton;
    [SerializeField] GameObject sfxSlider;
    [SerializeField] Sprite soundOnSprite;
    [SerializeField] Sprite soundOffSprite;

    public bool isInTDMode = true;
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
            menuButton.onClick.AddListener(ToggleMenu);
        }

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
        audioManager.playButtonClickSFX();
        if (isMenuOpen)
        {
            // If opening the menu, just make it visible and interactable
            menuGameObject.SetActive(true);
            if (isInTDMode){
                LevelManager.SaveGameSpeed();
                LevelManager.SetGameSpeed(0);
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

    public void BackToMenu(){
        audioManager.playButtonClickSFX();
        audioManager.StartMusicForVillageMode();
        SceneManager.LoadScene("MainMenu");
    }

    public void BackToIdle(){
        audioManager.playButtonClickSFX();
        audioManager.StartMusicForVillageMode();
        SceneManager.LoadScene("MainMenu");
    }

    public void TestFinishLevel(){
        LevelManager.main.ExitToMainMenu(true); 
    }

    public void PlayNextTDSong(){
         audioManager.PlayRandomTDTrack();
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
        audioManager.playButtonClickSFX();
        audioManager.StartMusicForTDMode();
        SceneManager.LoadScene(currentScene.name);
    }
}
