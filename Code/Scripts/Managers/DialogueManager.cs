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
                audioManager.PlaySFX(audioManager.dialogNextSFX);
            }
            else
            {
                // Last dialogue; hide the dialogue box
                TriggerPopDown();
            }
        }
    }

    public void TriggerPopUp()
    {
        
        isDialogueActive = true;
        FridgePopUpAnimator.gameObject.SetActive(true);
        FridgePopUpAnimator.SetTrigger("PopUp");
        currentDialogueIndex = 0; // Reset dialogue index
        UpdateDialogueText();

        audioManager.PlaySFX(audioManager.dialogUpSFX);
    }

    private void TriggerPopDown()
    {
        FridgePopUpAnimator.SetTrigger("PopDown"); // Ensure you have a PopDown animation
        isDialogueActive = false;
        
        audioManager.PlaySFX(audioManager.dialogDownSFX);
    }

    private void UpdateDialogueText()
    {
        if (dialogueText != null && dialogues.Length > 0)
        {
            dialogueText.text = dialogues[currentDialogueIndex];
        }
    }
}
