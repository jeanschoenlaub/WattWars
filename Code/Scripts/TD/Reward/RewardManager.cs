using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class RewardManager : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] public GameObject mainCanvas;
    [SerializeField] public Animator cameraAnimator;
     

    //Singletons
    private AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindWithTag("Audio").GetComponent<AudioManager>();
    }

    public void startRewardTransition() {
        mainCanvas.SetActive(false);
        cameraAnimator.SetTrigger("MoveUp");
    }
}