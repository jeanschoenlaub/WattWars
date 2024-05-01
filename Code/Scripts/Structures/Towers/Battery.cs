using UnityEngine;

public class Battery : MonoBehaviour
{
    [Header("Battery Sprites")]
    [SerializeField] Sprite sprite0;
    [SerializeField] Sprite sprite10;
    [SerializeField] Sprite sprite20;
    [SerializeField] Sprite sprite30;
    [SerializeField] Sprite sprite40;
    [SerializeField] Sprite sprite50;
    [SerializeField] Sprite sprite60;
    [SerializeField] Sprite sprite70;
    [SerializeField] Sprite sprite80;
    [SerializeField] Sprite sprite90;
    [SerializeField] Sprite sprite100;
    [SerializeField] GameObject SpriteToReplace;

    public void SetSprite(float currentCharge, float maxCharge)
    {
        // Calculate the charge percentage
        float chargePercentage = currentCharge / maxCharge * 100f;

        // Determine which sprite to use based on charge percentage
        Sprite selectedSprite = null;
        if (chargePercentage <= 0f){selectedSprite = sprite0;}
        else if (chargePercentage <= 10f){selectedSprite = sprite10;}
        else if (chargePercentage <= 20f){selectedSprite = sprite20;}
        else if (chargePercentage <= 30f){selectedSprite = sprite30;}
        else if (chargePercentage <= 40f){selectedSprite = sprite40;}
        else if (chargePercentage <= 50f){selectedSprite = sprite50;}
        else if (chargePercentage <= 60f){selectedSprite = sprite60;}
        else if (chargePercentage <= 70f){selectedSprite = sprite70;}
        else if (chargePercentage <= 80f){selectedSprite = sprite80;}
        else if (chargePercentage <= 90f){selectedSprite = sprite90;}
        else if (chargePercentage <= 100f){selectedSprite = sprite100;}
       
        // Set the sprite
        if (selectedSprite != null)
        {
            SpriteToReplace.GetComponent<SpriteRenderer>().sprite = selectedSprite;
        }
        else
        {
            Debug.LogWarning("No appropriate sprite found for the current charge.");
        }
    }
}