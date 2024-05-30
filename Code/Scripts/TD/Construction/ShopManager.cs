using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using TMPro;
using UnityEngine.EventSystems;

public class ShopManager : MonoBehaviour
{
    public static ShopManager main;

    [Header("Shop Buttons and Cooldowns")]
    // Even if use buttons, we use triggers on pointer up and down for drag and drop functionalities
    [SerializeField] private Button[] structureButtons; // overlay image must be last child

    private bool[] isOnCooldown;

    private void Awake()
    {
        main = this;

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

    // Function to let building manager create a stucture preview, also used by plot script
    private void SelectStructure(int index)
    {
        BuildManager.main.SetSelectedStructure(index); // True for tower
    }

    // Function to set a flag 
    public void StartCooldown(Structure structure)
    {
        // Find the corresponding button and index for the given structure
        int index = Array.FindIndex(structureButtons, b => b.GetComponent<StructRefShop>().structure == structure);
        if (index == -1){ Debug.LogError("Structure not found in structureButtons array."); return;}
        Button button = structureButtons[index];

        //And then starts a coroutine to visually show the cooldown poeriod over the shop image
        isOnCooldown[index] = true; // Set the cooldown state to true
        Image cooldownOverlay = button.transform.GetChild(button.transform.childCount - 1).GetComponent<Image>();
        StartCoroutine(CooldownRoutine(button, cooldownOverlay, structure.buildCooldown, index));
    }

    private IEnumerator CooldownRoutine(Button button, Image cooldownOverlay, float cooldownTime, int index)
    {
        button.interactable = false; // Not used functionally, but visually to color the button
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
        button.interactable = true; // Visaually color the button back
    }

    void SetupButtonInteractions(Button button, int buttonIndex, Structure structure)
    {
        AddEventTriggers(button, buttonIndex, structure);
    }

    void AddEventTriggers(Button button, int index, Structure structure)
    {
        EventTrigger trigger = button.GetComponent<EventTrigger>() ?? button.gameObject.AddComponent<EventTrigger>();
        trigger.triggers.Clear(); // Clear previous triggers to avoid duplicate actions

        EventTrigger.Entry pointerDownEntry = new EventTrigger.Entry 
        {
            eventID = EventTriggerType.PointerDown,
            callback = new EventTrigger.TriggerEvent()
        };
        pointerDownEntry.callback.AddListener((data) => {
            if (LevelManager.main.GetCurrentMoney() >= structure.cost){
                if (!isOnCooldown[index]){
                    SelectStructure(index);
                }
            }        
        });
        trigger.triggers.Add(pointerDownEntry);

        // Add a Pointer Up that will be triggereing build on release
        EventTrigger.Entry pointerUpEntry = new EventTrigger.Entry 
        {
            eventID = EventTriggerType.PointerUp,
            callback = new EventTrigger.TriggerEvent()
        };
        // We use pointer up world position to trigger Grid to fetch the correct Plot script
        pointerUpEntry.callback.AddListener((data) => {
            PointerEventData eventData = (PointerEventData)data;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(eventData.position);
            GridManager.Instance.PlaceStructureAtPosition(worldPosition);
        });
        trigger.triggers.Add(pointerUpEntry);
    }
}
