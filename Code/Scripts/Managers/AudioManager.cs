using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("SFX")]
    public AudioClip Track1;
    public AudioClip coinsSFX;
    public AudioClip dialogSFX;
    public AudioClip badActionSFX;
    public AudioClip buildSFX;
    public AudioClip buildSelected;

    public void Start() {
        musicSource.clip = Track1;
        musicSource.Play();
    }
    
    //function to call any oneshot clip
    public void PlaySFX( AudioClip clip){
        Debug.Log("called music");
        SFXSource.PlayOneShot(clip);
    }
    
}
