using System.Collections.Generic;
using UnityEngine;
using System.Collections;

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
        StartCoroutine(WaitAnDstartScientistDialog());
    }

    IEnumerator WaitAnDstartScientistDialog(){
        yield return new WaitForSeconds(1.5f);
        List<string> dialogQueue1 = new List<string>
        {
            "To repair the breaker box, we need to turn each of the switches from <color=#c84f4f>OFF</color> to <color=#32C142>ON</color>.",
            "Click on the first switch to turn it on!",
        };
        dialogueManager.StartDialog(dialogQueue1);
        PlayerPrefs.SetInt("QuestProgress", 3);
    }
}