using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.EventSystems;

public class ShopManager : MonoBehaviour
{
    [Header("Shop Buttons and Cooldowns")]
    [SerializeField] private Button[] structureButtons; // overlay image must be last child

    private bool[] isOnCooldown;

    private void Awake()
    {
        isOnCooldown = new bool[structureButtons.Length];
        for (int i = 0; i < structureButtons.Length; i++)
        {
            // Get the structure attached to each button via the StructRefShop script
            var structureRef = structureButtons[i].GetComponent<StructRefShop>();
            // And the cost test that is placed on child -2 pos (overlay is last child)
            TextMeshProUGUI towerCostTextUI = structureButtons[i].transform.GetChild(structureButtons[i].transform.childCount - 2).GetComponent<TextMeshProUGUI>();


            if (structureRef != null && towerCostTextUI != null && structureRef.structure != null)
            {
                towerCostTextUI.text = structureRef.structure.cost.ToString();
                
                SetupButtonInteractions(structureButtons[i], i, structureRef.structure);
            }

            else
            {
                Debug.LogError("No structure assigned to button " + structureButtons[i].name + "or missing cost text");
            }
        }
    }

    private void Update() {
        //Disable buttons if not enough money 
        var currentMoney = LevelManager.main.GetCurrentMoney();
        for (int i = 0; i < structureButtons.Length; i++)
        {
            var structure = structureButtons[i].GetComponent<StructRefShop>().structure;
            if (structure.cost > currentMoney || isOnCooldown[i]){
                structureButtons[i].interactable = false;
            }
            else {structureButtons[i].interactable = true;}
        }
    }

    private void SelectStructure(int index, Structure structure)
    {
        Debug.Log(structure.structureName);
        BuildManager.main.SetSelectedStructure(index); // True for tower
        StartCooldown(structureButtons[index], index, structure.buildCooldown);
    }

    private void StartCooldown(Button button, int index, float cooldownTime)
    {
        // the last child of each button is the cooldown overlay
        Image cooldownOverlay = button.transform.GetChild(button.transform.childCount - 1).GetComponent<Image>();
        isOnCooldown[index] = true; // Set the cooldown state to true
        StartCoroutine(CooldownRoutine(button, cooldownOverlay, cooldownTime, index));
    }

    private IEnumerator CooldownRoutine(Button button, Image cooldownOverlay, float cooldownTime, int index)
    {
        button.interactable = false;
        float remainingTime = cooldownTime;

        while (remainingTime > 0)
        {
            int currentGameSpeed = LevelManager.GetGameSpeed();
            cooldownOverlay.fillAmount = remainingTime / cooldownTime;
            remainingTime -= Time.deltaTime * currentGameSpeed;
            yield return null;
        }

        cooldownOverlay.fillAmount = 0;
        isOnCooldown[index] = false; // Reset the cooldown state
        button.interactable = true;
    }

    void SetupButtonInteractions(Button button, int buttonIndex, Structure structure)
    {
        // Local index for lambda func
        int localIndex = buttonIndex;
        Structure localStructure = structure;
        // Assign the select function directly for clicks
        //button.onClick.AddListener(() => SelectStructure(localIndex, localStructure));

        // Assign the event trigger with direct capture of parameters
        AddEventTriggers(button, buttonIndex, structure);
    }

    void AddEventTriggers(Button button, int index, Structure structure)
    {
        EventTrigger trigger = button.GetComponent<EventTrigger>() ?? button.gameObject.AddComponent<EventTrigger>();
        trigger.triggers.Clear(); // Clear previous triggers to avoid duplicate actions

        EventTrigger.Entry pointerDownEntry = new EventTrigger.Entry();
        pointerDownEntry.eventID = EventTriggerType.PointerDown;
        pointerDownEntry.callback.AddListener((data) => {
            Debug.Log($"Pointer down on structure: {structure.name}");
            SelectStructure(index, structure);
        });
        trigger.triggers.Add(pointerDownEntry);

        // Add Pointer Up event
        EventTrigger.Entry pointerUpEntry = new EventTrigger.Entry();
        pointerUpEntry.eventID = EventTriggerType.PointerUp;
        pointerUpEntry.callback.AddListener((data) => {
            PointerEventData eventData = (PointerEventData)data;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(eventData.position);
            worldPosition.z = 0; // Assuming your game is 2D and plots are on z = 0
            Debug.Log($"Pointer up on structure: {structure.name}, world position: {worldPosition}");

            GridManager.Instance.PlaceStructureAtPosition(worldPosition, structure);
        });
        trigger.triggers.Add(pointerUpEntry);
    }

}
