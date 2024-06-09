using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("Tracks")]
    public AudioClip villageAmbience;
    public AudioClip tdTrack1;
    public AudioClip tdTrack2;
    public AudioClip tdTrack3;
    public AudioClip tdTrack4;
    public AudioClip tdTrack5;

    [Header("SFX")]
    public AudioClip buttonClick;

    [Header("TD Tower SFX")]
    public AudioClip coinsSFX;
    public AudioClip badActionSFX;
    public AudioClip buildSFX;
    public AudioClip buildSelected;
    public AudioClip ElectricEnemyHit;
    public AudioClip FuelEnemyHit;
    public AudioClip ButtonSwitch;
    public AudioClip dialogSFX;

    [Header("LvlSelectionSFX")]
    public AudioClip breakerSwitch;
    public AudioClip electricSound;
    public AudioClip rewardSound;

    private AudioClip[] tdTracks;

    public static AudioManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Make this object persistent
        }
        else if (Instance != this)
        {
            Destroy(gameObject);  // Destroy this instance because it is a duplicate
            return;
        }
        StartMusicForVillageMode();
    }

    public void StartMusicForVillageMode()
    {
        musicSource.clip = villageAmbience;
        musicSource.loop = true; // Enable looping
        musicSource.Play();
    }

    public void StartMusicForTDMode()
    {
        // Initialize the tracks array with the available tracks
        tdTracks = new AudioClip[] { tdTrack1, tdTrack2, tdTrack3, tdTrack4, tdTrack5 };
        musicSource.loop = false;
        musicSource.playOnAwake = false;
        PlayRandomTDTrack();
    }

    void Update()
    {
        // Check if the music source is not playing and play a new random track
        if (!musicSource.isPlaying)
        {
            PlayRandomTDTrack();
        }
    }

    // Function to play a random track
    public void PlayRandomTDTrack()
    {
        AudioClip newTrack = tdTracks[Random.Range(0, tdTracks.Length)];
        PlayTrack(newTrack);
    }

    // Function to change music
    public void PlayTrack(AudioClip newTrack)
    {
        if (musicSource.clip == newTrack)
        {
            // If the new track is the same as the currently playing track, do nothing
            return;
        }

        musicSource.Stop();      // Stop the current track if any
        musicSource.clip = newTrack; // Assign the new track to the music source
        musicSource.Play();      // Play the new track
    }
    
    public void MuteAllSound()
    {
        musicSource.volume = 0;
        SFXSource.volume = 0;
    }

    // Volume is expected to be between 0.0f (silent) and 1.0f (full volume)
    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    // Volume is expected to be between 0.0f (silent) and 1.0f (full volume)
    public void SetSFXVolume(float volume)
    {
        SFXSource.volume = volume;
    }

    public float GetCurrentVolume()
    {
        // Assuming musicSource and SFXSource are always set to the same volume,
        // we can just return the volume of one of them.
        return musicSource.volume;
    }
    
    //function to call any oneshot clip
    public void PlaySFX(AudioClip clip){
        SFXSource.PlayOneShot(clip);
    }

    // Method to start playing a looping sound effect
    public void PlayLoopingSFX(AudioClip clip)
    {
        SFXSource.clip = clip;
        SFXSource.loop = true; // Enable looping
        SFXSource.Play();
    }

    // Method to stop the currently playing looping sound effect (dosesn't stop sound straight away though)
    public void StopLoopingSFX()
    {
       SFXSource.loop = false; // Enable looping
    }

    public void playButtonClickSFX(){
        PlaySFX(buttonClick);
    }


    
}
