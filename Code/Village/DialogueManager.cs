
using UnityEngine;
using UnityEngine.UI; // Required for UI elements
using TMPro;
using System.Collections;

// Tutorial to show the player how to place a fuel and a diesel generator tower 
public class DialogueManager : MonoBehaviour
{
    [SerializeField] public Animator FridgeDialogAnimator; // To make mouse move up and down to direct user

    [Header("Text References")]
    [SerializeField] public Button DialogFridgeClickDetector;

    // SingleTons
    private AudioManager audioManager;

    private void Awake()
    {
        // Get the TutoPlaceTower component attached to the same GameObject
        audioManager = GameObject.FindWithTag("Audio").GetComponent<AudioManager>();
    }

    public void DialogPopUp(){
        Debug.Log("Dia");
        StartCoroutine(DialogPopUpAndWait());
    }

    public void DialogPopDown(){
        FridgeDialogAnimator.SetTrigger("PopDown");
        audioManager.PlaySFX(audioManager.dialogSFX);
    }

    IEnumerator DialogPopUpAndWait()
    {
        audioManager.PlaySFX(audioManager.dialogSFX);
        FridgeDialogAnimator.SetTrigger("PopUp");

        // And after little delay add a listener for the next step
        yield return new WaitForSeconds(0.5f);
        DialogFridgeClickDetector.onClick.AddListener(DialogPopDown);
    }
}