using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private Animator dialogAnimator;
    [SerializeField] private Button dialogClickDetectorButton;
    [SerializeField] private GameObject dialogClickDetector;
    [SerializeField] private TextMeshProUGUI dialogText;

    [Header("Quest Management")]
    [SerializeField] private GameObject QuestScientist1;
    [SerializeField] private GameObject QuestScientist2;

    [Header("BB Management")]
    [SerializeField] private Button BreakerBox1;
    [SerializeField] private GameObject UnlockBB1AnimGO;

    private AudioManager audioManager;

   private void Awake()
    {
        audioManager = GameObject.FindWithTag("Audio").GetComponent<AudioManager>();
    }

    public void StartScientist1Dialog()
    {
        List<string> dialogQueue1 = new List<string>
        {
            "Welcome ! You must be the new engineer sent to bring <color=#F5FF00>Power</color> back to this town.",
            "I've installed a small <color=#85282B>Diesel Generator </color>to get you started. Now we need to connect it, follow me!"
        };
        StartCoroutine(StartDialog(dialogQueue1, () =>
        {
            // This code runs after the first dialog sequence finishes
            QuestScientist1.SetActive(false);
            PlayerPrefs.SetInt("QuestProgress", 1);
            QuestScientist2.SetActive(true);
        }));
    }

    public void StartScientist2Dialog()
    {
        List<string> dialogQueue2 = new List<string>
        {
            "Because these damn AI have infected all our power equipment we need to restart them...",
            "Open the orange breaker box, I'll show you how to restart this one!"
        };
        StartCoroutine(StartDialog(dialogQueue2, () =>
        {
            // This code runs after the second dialog sequence finishes
            TriggerUnlockAnimation();
            QuestScientist2.SetActive(false);
            PlayerPrefs.SetInt("QuestProgress", 2);
            BreakerBox1.interactable = true;
        }));
    }

    public void TriggerUnlockAnimation(){
        Animator unlockAnimator = UnlockBB1AnimGO.GetComponent<Animator>();
        unlockAnimator.SetTrigger("UnlockNextBreaker");
    }

    private IEnumerator StartDialog(List<string> dialogQueue, Action onComplete)
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