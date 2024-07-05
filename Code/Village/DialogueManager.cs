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
    [SerializeField] private List<string> dialogQueue2;

    [Header("Quest Management")]
    [SerializeField] private GameObject QuestScientist1;
    [SerializeField] private GameObject QuestScientist2;
    

    private AudioManager audioManager;
    private int currentDialogIndex = 0;

        private void Awake()
    {
        audioManager = GameObject.FindWithTag("Audio").GetComponent<AudioManager>();
    }

    public void StartDialogScientist1()
    {
        dialogAnimator.SetBool("isDialogueOpen",true);
        if (dialogQueue.Count > 0)
        {
            currentDialogIndex = 0;
            StartCoroutine(DialogScientist1PopUpAndWait());
        }
    }

    public void StartDialogScientist2()
    {
        dialogAnimator.SetBool("isDialogueOpen",true);
        if (dialogQueue2.Count > 0)
        {
            currentDialogIndex = 0;
            StartCoroutine(DialogScientist2PopUpAndWait());
        }
    }

    public void DialogPopDown()
    {
        dialogAnimator.SetTrigger("PopDown");
        audioManager.PlaySFX(audioManager.dialogSFX);
    }

    private IEnumerator DialogScientist1PopUpAndWait()
    {
        while (currentDialogIndex < dialogQueue.Count)
        {
            if (currentDialogIndex == 0) { audioManager.PlaySFX(audioManager.dialogSFX); }
            dialogAnimator.SetTrigger("PopUp");

            dialogText.text = dialogQueue[currentDialogIndex];
            currentDialogIndex++;

            yield return new WaitForSeconds(0.5f);

            // Wait until the button is clicked
            bool buttonClicked = false;
            dialogClickDetector.onClick.RemoveAllListeners();
            dialogClickDetector.onClick.AddListener(() => buttonClicked = true);

            yield return new WaitUntil(() => buttonClicked);
        }

        QuestScientist1.SetActive(false);
        PlayerPrefs.SetInt("QuestProgress", 1); // Get the quest progress
        QuestScientist2.SetActive(true);
        dialogAnimator.SetBool("isDialogueOpen",false);
        DialogPopDown();
    }


     private IEnumerator DialogScientist2PopUpAndWait()
    {
        while (currentDialogIndex < dialogQueue2.Count)
        {
            if (currentDialogIndex == 0) { audioManager.PlaySFX(audioManager.dialogSFX); }
            dialogAnimator.SetTrigger("PopUp");

            dialogText.text = dialogQueue2[currentDialogIndex];
            currentDialogIndex++;

            yield return new WaitForSeconds(0.5f);

            // Wait until the button is clicked
            bool buttonClicked = false;
            dialogClickDetector.onClick.RemoveAllListeners();
            dialogClickDetector.onClick.AddListener(() => buttonClicked = true);

            yield return new WaitUntil(() => buttonClicked);


        }

        QuestScientist2.SetActive(false);
        dialogAnimator.SetBool("isDialogueOpen",false);
        PlayerPrefs.SetInt("QuestProgress", 2); // Get the quest progress
        DialogPopDown();
    }
}
