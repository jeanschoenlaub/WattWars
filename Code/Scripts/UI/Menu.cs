using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Menu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] TextMeshProUGUI currencyUI;
    [SerializeField] TextMeshProUGUI livesUI;
    [SerializeField] TextMeshProUGUI waveUI;
    [SerializeField] TextMeshProUGUI waveAnim;
    [SerializeField] Animator anim;

    private bool isMenuOpen = false;
    private bool isPlaying = true; // Assuming the game starts in play mode
    private bool isFastForwarding = false; // Track the fast-forward state

    public Button playPauseButton;
    public Button ffButton; // Reference to the Fast Forward button
    public Sprite playSprite;
    public Sprite pauseSprite;

    private WaveManager waveManager;

    void Start()
    {
        waveManager = FindObjectOfType<WaveManager>();
        playPauseButton.onClick.AddListener(TogglePlayPause);
        ffButton.onClick.AddListener(ToggleFastForward);
        UpdateFFButtonColor(); // Set initial FF button color
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
        anim.SetBool("MenuOpen", isMenuOpen);
    }

    private void OnGUI(){
        currencyUI.text = LevelManager.main.coins.ToString();
        livesUI.text = LevelManager.main.currentScenario.Lives.ToString();

        //For the following waves starts at 0 and we want to start with 1 for UI 
        waveAnim.text =  "Day "+ (waveManager.currentDayIndex + 1).ToString() + " - Wave " + (waveManager.currentWaveIndex + 1).ToString();
        waveUI.text =   "Wave " + (waveManager.currentWaveIndex + 1).ToString() + "/" 
                        +  waveManager.currentDay.waves.Count.ToString();
    }

    

}
