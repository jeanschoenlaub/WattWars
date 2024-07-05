using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DialogueManagerVillage : MonoBehaviour
{
    [Header("Dialogue Management")]
    [SerializeField] private DialogueManager dialogueManager;

    [Header("Quest Management")]
    [SerializeField] private GameObject QuestScientist1;
    [SerializeField] private GameObject QuestScientist2;

    [Header("BB Management")]
    [SerializeField] private Button BreakerBox1;
    [SerializeField] private GameObject UnlockBB1AnimGO;

    private void Awake()
    {
        // Ensure dialogueManager is assigned, or find it on the same GameObject
        if (dialogueManager == null)
        {
            Debug.LogWarning("DialogueManager is not assigned in on " + gameObject.name);
        }
    }

    public void StartScientist1Dialog()
    {
        List<string> dialogQueue1 = new List<string>
        {
            "Welcome ! You must be the new engineer sent to bring <color=#F5FF00>Power</color> back to this town.",
            "I've installed a small <color=#85282B>Diesel Generator </color>to get you started. Now we need to connect it, follow me!"
        };
        dialogueManager.StartDialog(dialogQueue1, onComplete: () =>
        {
            // This code runs after the first dialog sequence finishes
            QuestScientist1.SetActive(false);
            PlayerPrefs.SetInt("QuestProgress", 1);
            QuestScientist2.SetActive(true);
        });
    }

    public void StartScientist2Dialog()
    {
        List<string> dialogQueue2 = new List<string>
        {
            "Because these damn AI have infected all our power equipment we need to restart them...",
            "Open the orange breaker box, I'll show you how to restart this one!"
        };
         dialogueManager.StartDialog(dialogQueue2, onComplete: () =>
        {
            // This code runs after the second dialog sequence finishes
            TriggerUnlockAnimation();
            QuestScientist2.SetActive(false);
            PlayerPrefs.SetInt("QuestProgress", 2);
            BreakerBox1.interactable = true;
        });
    }

    public void TriggerUnlockAnimation(){
        Animator unlockAnimator = UnlockBB1AnimGO.GetComponent<Animator>();
        unlockAnimator.SetTrigger("UnlockNextBreaker");
    }
}