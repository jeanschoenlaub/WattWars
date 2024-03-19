using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Menu : MonoBehaviour
{
    [Header("--------- Score and UI ---------")]
    [SerializeField] TextMeshProUGUI currencyUI;
    [SerializeField] TextMeshProUGUI livesUI;
    [SerializeField] TextMeshProUGUI waveUI;
    [SerializeField] TextMeshProUGUI waveAnim;
    

    [Header("--------- GameSpeed and Menu ---------")]
    [SerializeField] Animator menuAnim;
    [SerializeField] Button playPauseButton;
    [SerializeField] Button ffButton; 
    [SerializeField] Sprite playSprite;
    [SerializeField] Sprite pauseSprite;
    private bool isMenuOpen = false;
    private bool isPlaying = true; 
    private bool isFastForwarding = false; 
    
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

        playPauseButton.onClick.AddListener(TogglePlayPause);
        ffButton.onClick.AddListener(ToggleFastForward);
        UpdateFFButtonColor(); // Set initial FF button color

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

    void TogglePlayPause()
    {
        if (isPlaying)
        {
            playPauseButton.image.sprite = pauseSprite;
            LevelManager.SetGameSpeed(0); // Pause the game
        }
        else
        {
            playPauseButton.image.sprite = playSprite;
            LevelManager.SetGameSpeed(isFastForwarding ? 2 : 1); // Resume the game, consider fast-forward state
        }
        isPlaying = !isPlaying;
        ToggleMenu();
    }

    void ToggleFastForward()
    {
        if (isFastForwarding)
        {
            // If currently fast forwarding, set to normal play speed
            LevelManager.SetGameSpeed(isPlaying ? 1 : 0); // Adjust speed based on play/pause state
            isFastForwarding = false;
        }
        else
        {
            // If not fast forwarding, set to fast forward speed
            if (isPlaying) // Only increase speed if the game is playing
            {
                LevelManager.SetGameSpeed(2);
            }
            isFastForwarding = true;
        }
        UpdateFFButtonColor(); // Update the FF button color based on its state
    }

    void UpdateFFButtonColor()
    {
        // Set FF button color based on its state
        ffButton.image.color = isFastForwarding ? new Color(0, 0, 0.5f, 1) : Color.white; // Dark navy blue or white
    }

    public void ToggleMenu(){
        isMenuOpen = !isMenuOpen;
        menuAnim.SetBool("MenuOpen", isMenuOpen);
    }

    private void OnGUI(){
        currencyUI.text = LevelManager.main.coins.ToString();
        livesUI.text = LevelManager.main.currentScenario.Lives.ToString();

        if (waveAnim && waveUI){
             //For the following waves starts at 0 and we want to start with 1 for UI 
        waveAnim.text =  "Day "+ (waveManager.currentDayIndex + 1).ToString() + " - Wave " + (waveManager.currentWaveIndex + 1).ToString();
        waveUI.text =   "Wave " + (waveManager.currentWaveIndex + 1).ToString() + "/" 
                        +  waveManager.currentDay.waves.Count.ToString();

        }else{
            Debug.Log("UI Text Missing");
        }
    }

    

}
