using UnityEngine;
using UnityEngine.UI;

public class PlayPauseButton : MonoBehaviour
{
    public Button playPauseButton;
    public Sprite playSprite;
    public Sprite pauseSprite;

    private bool isPlaying = true; // Assuming the game starts in play mode

    void Start()
    {
        playPauseButton.onClick.AddListener(TogglePlayPause);
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
            LevelManager.SetGameSpeed(1); // Resume the game
        }
        isPlaying = !isPlaying;
    }
}
