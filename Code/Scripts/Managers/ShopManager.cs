using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

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
            int index = i; // Local copy of index for the lambda capture

            // Get the structure attached to each button via the StructRefShop script
            var structureRef = structureButtons[i].GetComponent<StructRefShop>();
            // And the cost test that is placed on child -2 pos (overlay is last child)
            TextMeshProUGUI towerCostTextUI = structureButtons[i].transform.GetChild(structureButtons[i].transform.childCount - 2).GetComponent<TextMeshProUGUI>();


            if (structureRef != null && towerCostTextUI != null && structureRef.structure != null)
            {
               structureButtons[i].onClick.AddListener(() => SelectStructure(index, structureRef.structure));
               towerCostTextUI.text = structureRef.structure.cost.ToString();
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
}
