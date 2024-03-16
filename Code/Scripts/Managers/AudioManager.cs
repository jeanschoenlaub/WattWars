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

    public void Start() {
        musicSource.clip = Track1;
        musicSource.Play();
    }
    
    //function to call any oneshot clip
    public void PlaySFX( AudioClip clip){
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
