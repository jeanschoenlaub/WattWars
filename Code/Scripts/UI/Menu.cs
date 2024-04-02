using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [Header("--------- Score and UI ---------")]
    [SerializeField] TextMeshProUGUI currencyUI;
    [SerializeField] TextMeshProUGUI livesUI;
    [SerializeField] TextMeshProUGUI waveUI;
    [SerializeField] TextMeshProUGUI waveAnim;
    

    [Header("--------- GameSpeed and Menu ---------")]
    [SerializeField] Button menuButton;
    [SerializeField] private GameObject menuGameObject;
    [SerializeField] Sprite playSprite;
    [SerializeField] Sprite pauseSprite;
    private bool isMenuOpen = false;

    
    [Header("--------- GameSound ---------")]
    [SerializeField] Button soundButton;
    [SerializeField] GameObject volumeSlider;
    [SerializeField] Sprite soundOnSprite;
    [SerializeField] Sprite soundOffSprite;

    private WaveManager waveManager;
    private AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindWithTag("Audio").GetComponent<AudioManager>();
    }   

    void Start()
    {
        waveManager = FindObjectOfType<WaveManager>();
        menuGameObject.SetActive(false);

        menuButton.onClick.AddListener(ToggleMenu);
        soundButton.onClick.AddListener(ToggleAudio);
        volumeSlider.GetComponent<Slider>().onValueChanged.AddListener((value) => audioManager.SetVolume(value));
    }

    void ToggleAudio()
    {
        Slider volumeSliderComponent = volumeSlider.GetComponent<Slider>();

        // If volume is not 0, mute the sound
        if (audioManager.GetCurrentVolume() > 0)
        {
            // Remember current volume to restore it later
            PlayerPrefs.SetFloat("LastVolume", audioManager.GetCurrentVolume());

            // Mute the sound
            volumeSliderComponent.value = 0;
            soundButton.image.sprite = soundOffSprite;
            audioManager.SetVolume(0);
        }
        else
        {
            // Restore the previous volume, or set it to a default value if not available
            float lastVolume = PlayerPrefs.GetFloat("LastVolume", 0.5f); // Default to 0.5 if no last volume saved

            // Unmute the sound to the last volume or default
            volumeSliderComponent.value = lastVolume;
            soundButton.image.sprite = soundOnSprite;
            audioManager.SetVolume(lastVolume);
        }
    }


    public void ToggleMenu(){
        isMenuOpen = !isMenuOpen;
        LevelManager.SetGameSpeed(isMenuOpen ? 0 : 1);
        
        if (isMenuOpen)
        {
            // If opening the menu, just make it visible and interactable
            menuGameObject.SetActive(true);
        }
        else
        {
            // If closing the menu, first make all buttons non-interactable,
            // then make the menu itself inactive
            menuGameObject.SetActive(false);
        }
    }

    private void OnGUI(){
        currencyUI.text = LevelManager.main.coins.ToString();
        livesUI.text = LevelManager.main.GetNumeberOfLives().ToString();

        if (waveAnim && waveUI){
             //For the following waves starts at 0 and we want to start with 1 for UI 
        waveAnim.text =  "Day "+ (waveManager.currentDayIndex + 1).ToString() + " - Wave " + (waveManager.currentWaveIndex + 1).ToString();
        waveUI.text =   "Wave " + (waveManager.currentWaveIndex + 1).ToString() + "/" 
                        +  waveManager.currentDay.waves.Count.ToString();

        }else{
            Debug.Log("UI Text Missing");
        }
    }

    public void BackToMenu(){
        SceneManager.LoadScene("Menu");
    }

    public void Restart(){
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    

}
