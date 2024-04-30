using UnityEngine;

public class Battery : MonoBehaviour
{
    [Header("Battery Sprites")]
    [SerializeField] Sprite sprite0;
    [SerializeField] Sprite sprite15;
    [SerializeField] Sprite sprite30;
    [SerializeField] Sprite sprite50;
    [SerializeField] Sprite sprite70;
    [SerializeField] Sprite sprite85;
    [SerializeField] Sprite sprite90;
    [SerializeField] GameObject SpriteToReplace;

    public void SetSprite(float currentCharge, float maxCharge)
    {
        Debug.Log("called");
        // Calculate the charge percentage
        float chargePercentage = currentCharge / maxCharge * 100f;

        // Determine which sprite to use based on charge percentage
        Sprite selectedSprite = null;

        if (chargePercentage <= 0f)
        {
            selectedSprite = sprite0;
        }
        else if (chargePercentage <= 15f)
        {
            selectedSprite = sprite15;
        }
        else if (chargePercentage <= 30f)
        {
            selectedSprite = sprite30;
        }
        else if (chargePercentage <= 50f)
        {
            selectedSprite = sprite50;
        }
        else if (chargePercentage <= 70f)
        {
            selectedSprite = sprite70;
        }
        else if (chargePercentage <= 85f)
        {
            selectedSprite = sprite85;
        }
        else
        {
            selectedSprite = sprite90;
        }

        // Set the sprite
        if (selectedSprite != null)
        {
            // Assuming you have a SpriteRenderer component attached to this GameObject
            SpriteToReplace.GetComponent<SpriteRenderer>().sprite = selectedSprite;
        }
        else
        {
            Debug.LogWarning("No appropriate sprite found for the current charge.");
        }
    }

}