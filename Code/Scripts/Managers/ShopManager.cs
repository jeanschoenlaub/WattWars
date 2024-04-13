using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//TO-DO
// Disabel component if not enough money here to prevent cool down and nno buidl
// Get cost text and update from here
// Wrong button bug on 2 and 3  

public class ShopManager : MonoBehaviour
{
    [Header("Shop Buttons and Cooldowns")]
    [SerializeField] private Button[] structureButtons; // overlay image must be last child

    private void Awake()
    {
        for (int i = 0; i < structureButtons.Length; i++)
        {
            // Get the structure attached to each button via the StructRefShop script
            var structureRef = structureButtons[i].GetComponent<StructRefShop>();
            int index = i; // Local copy of index for the lambda capture

            if (structureRef != null && structureRef.structure != null)
            {
               structureButtons[i].onClick.AddListener(() => SelectStructure(index, structureRef.structure));
            }
            else
            {
                Debug.LogError("No structure assigned to button " + structureButtons[i].name);
            }
        }
    }

    private void SelectStructure(int index, Structure structure)
    {
        BuildManager.main.SetSelectedStructure(index); // True for tower
        StartCooldown(structureButtons[index], index, structure.buildCooldown);
    }

    private void StartCooldown(Button button, int index, float cooldownTime)
    {
        // Assuming the last child is the cooldown overlay
        Image cooldownOverlay = button.transform.GetChild(button.transform.childCount - 1).GetComponent<Image>();
        StartCoroutine(CooldownRoutine(button, cooldownOverlay, cooldownTime));
    }

    private IEnumerator CooldownRoutine(Button button, Image cooldownOverlay, float cooldownTime)
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
        button.interactable = true;
    }
}
