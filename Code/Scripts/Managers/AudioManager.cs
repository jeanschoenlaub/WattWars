using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("Tracks")]
    public AudioClip Track1;
    public AudioClip QuizTrack;

    [Header("SFX")]
    public AudioClip coinsSFX;
    public AudioClip coinsPouringSFX;
    public AudioClip dialogNextSFX;
    public AudioClip dialogUpSFX;
    public AudioClip dialogDownSFX;
    public AudioClip badActionSFX;
    public AudioClip buildSFX;
    public AudioClip buildSelected;

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

        InitializeAudio();
    }

    void InitializeAudio()
    {
        musicSource.clip = Track1;
        musicSource.loop = true; // Enable looping
        musicSource.Play();
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


    
}
