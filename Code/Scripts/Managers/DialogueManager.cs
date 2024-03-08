using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public Animator FridgePopUpAnimator;
    [SerializeField] TextMeshProUGUI dialogueText; // Assign this in the inspector
    private bool isDialogueActive = false;
    private string[] dialogues; // Array to hold your dialogues
    private int currentDialogueIndex = 0;

    private AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindWithTag("Audio").GetComponent<AudioManager>();
    }

    void Start()
    {
        // Initialize your dialogues here, or set them from somewhere else
        dialogues = new string[] {
            "[Figidair] \n Graaar, me want oiiiil, give me that sweet soothing liquid !  \n\n (Click anywhere to continue)",
            "Dialogue 2 text here...",
            "Dialogue 3 text here..."
            // Add as many dialogues as you need
        };
    }

    public void OnDialogueAreaClicked()
    {
        Debug.Log("clicked");
        if (!isDialogueActive)
        {
            TriggerPopUp();
        }
        else
        {
            if (currentDialogueIndex < dialogues.Length - 1)
            {
                // Go to next dialogue
                currentDialogueIndex++;
                UpdateDialogueText();
                audioManager.PlaySFX(audioManager.dialogSFX);
            }
            else
            {
                // Last dialogue; hide the dialogue box
                Debug.Log("trigger pop down");
                TriggerPopDown();
            }
        }
    }

    public void TriggerPopUp()
    {
        Debug.Log("trigger pop-up");
        isDialogueActive = true;
        FridgePopUpAnimator.gameObject.SetActive(true);
        FridgePopUpAnimator.SetTrigger("PopUp");
        currentDialogueIndex = 0; // Reset dialogue index
        UpdateDialogueText();
    }

    private void TriggerPopDown()
    {
        FridgePopUpAnimator.SetTrigger("PopDown"); // Ensure you have a PopDown animation
        isDialogueActive = false;
        // Optionally reset currentDialogueIndex or wait until next TriggerPopUp
    }

    private void UpdateDialogueText()
    {
        if (dialogueText != null && dialogues.Length > 0)
        {
            dialogueText.text = dialogues[currentDialogueIndex];
        }
    }
}
