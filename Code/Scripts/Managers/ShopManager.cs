using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [Header("Shop Buttons and Cooldowns")]
    [SerializeField] private Button[] structureButtons; // Buttons for all structures
    [SerializeField] private Image[] cooldownOverlays; // Overlays that show cooldown

    [Header("Cooldown Settings")]
    [SerializeField] private float cooldownTime = 5f; // Time in seconds for the cooldown

    private void Awake()
    {
        // Assign button listeners
        for (int i = 0; i < structureButtons.Length; i++)
        {
            int index = i; // Local copy of index for the lambda capture
            structureButtons[i].onClick.AddListener(() => SelectStructure(index));
        }
    }

    private void SelectStructure(int index)
    {
        BuildManager.main.SetSelectedStructure(index); // True for tower
        //StartCooldown(structureButtons[index], index);
    }

    // private void StartCooldown(Button button, int index)
    // {
    //     StartCoroutine(CooldownRoutine(button, index));
    // }

    // private IEnumerator CooldownRoutine(Button button, int index)
    // {
    //     button.interactable = false;
    //     float remainingTime = cooldownTime;
    //     while (remainingTime > 0)
    //     {
    //         cooldownOverlays[index].fillAmount = remainingTime / cooldownTime;
    //         remainingTime -= Time.deltaTime;
    //         yield return null;
    //     }

    //     cooldownOverlays[index].fillAmount = 0;
    //     button.interactable = true;
    // }
}
