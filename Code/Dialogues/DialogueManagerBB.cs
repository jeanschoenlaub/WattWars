using System.Collections.Generic;
using UnityEngine;

public class DialogueManagerBB : MonoBehaviour
{
    [Header("Dialogue Management")]
    [SerializeField] private DialogueManager dialogueManager;

    private void Awake()
    {
        // Ensure dialogueManager is assigned, or find it on the same GameObject
        if (dialogueManager == null)
        {
            Debug.LogWarning("DialogueManager is not assigned in on " + gameObject.name);
        }
    }

    public void StartScientis1Dialog()
    {
        List<string> dialogQueue1 = new List<string>
        {
            "To repair the breaker box, we need to turn on the switches from<color=#c84f4f> OFF </color> to <color=#32C142>ON </color>.",
            "To repair the breaker box, we need to make all the switches green.",
        };
        dialogueManager.StartDialog(dialogQueue1);
        PlayerPrefs.SetInt("QuestProgress", 3);
    }
}