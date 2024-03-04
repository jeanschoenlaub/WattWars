using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("SFX")]
    public AudioClip coinsSFX;

    public void PlaySFX( AudioClip clip){
        SFXSource.PlayOneShot(clip);
    }
    
}
