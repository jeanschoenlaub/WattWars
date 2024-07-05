using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private Animator dialogAnimator;
    [SerializeField] private Button dialogClickDetector;
    [SerializeField] private TextMeshProUGUI dialogText;
    [SerializeField] private List<string> dialogQueue;

    private AudioManager audioManager;
    private int currentDialogIndex = 0;

    private void Awake()
    {
        // Get the TutoPlaceTower component attached to the same GameObject
        audioManager = GameObject.FindWithTag("Audio").GetComponent<AudioManager>();
    }

    public void DialogPopUp(){
        Debug.Log("pop");
        StartCoroutine(DialogPopUpAndWait());
    }

    public void DialogPopDown(){
        dialogAnimator.SetTrigger("PopDown");
        audioManager.PlaySFX(audioManager.dialogSFX);
    }

    IEnumerator DialogPopUpAndWait()
    {
        audioManager.PlaySFX(audioManager.dialogSFX);
        dialogAnimator.SetTrigger("PopUp");

        // And after little delay add a listener for the next step
        yield return new WaitForSeconds(0.5f);
        dialogClickDetector.onClick.AddListener(DialogPopDown);
    }
}
