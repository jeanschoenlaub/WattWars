using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] protected Animator dialogAnimator;
    [SerializeField] protected Button dialogClickDetectorButton;
    [SerializeField] protected GameObject dialogClickDetector;
    [SerializeField] protected TextMeshProUGUI dialogText;

    protected AudioManager audioManager;

    protected virtual void Awake()
    {
        audioManager = GameObject.FindWithTag("Audio").GetComponent<AudioManager>();
    }

    public void StartDialog(List<string> dialogQueue, Action onComplete = null)
    {
        if (dialogQueue.Count > 0)
        {
            StartCoroutine(DialogPopUpAndWait(dialogQueue, onComplete));
        }
    }

    protected IEnumerator DialogPopUpAndWait(List<string> dialogQueue, Action onComplete)
    {
        int currentDialogIndex = 0;
        dialogAnimator.SetBool("isDialogueOpen", true);
        dialogClickDetector.SetActive(true);

        while (currentDialogIndex < dialogQueue.Count)
        {
            if (currentDialogIndex == 0)
            {
                audioManager.PlaySFX(audioManager.dialogSFX);
            }

            dialogAnimator.SetTrigger("PopUp");
            dialogText.text = dialogQueue[currentDialogIndex];
            currentDialogIndex++;

            yield return new WaitForSeconds(0.5f);

            // Wait until the button is clicked
            bool buttonClicked = false;
            dialogClickDetectorButton.onClick.RemoveAllListeners();
            dialogClickDetectorButton.onClick.AddListener(() => buttonClicked = true);

            yield return new WaitUntil(() => buttonClicked);
        }

        dialogAnimator.SetBool("isDialogueOpen", false);
        DialogPopDown();
        onComplete?.Invoke();
    }

    public void DialogPopDown()
    {
        dialogAnimator.SetTrigger("PopDown");
        dialogClickDetector.SetActive(false);
        audioManager.PlaySFX(audioManager.dialogSFX);
    }
}