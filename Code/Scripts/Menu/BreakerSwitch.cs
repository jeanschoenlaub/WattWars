using UnityEngine;
using UnityEngine.EventSystems; // Required for UI event handling
using UnityEngine.UI; // Required for working with UI components

// Make sure your class implements the required interfaces for pointer events
public class HoverShaderSwitchUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Sprite defaultSprite; // Assign in the inspector
    public Sprite hoverSprite; // Assign in the inspector

    private Image imageComponent;

    void Start()
    {
        imageComponent = GetComponent<Image>();
        if (imageComponent != null)
        {
            imageComponent.sprite = defaultSprite;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (imageComponent != null)
        {
            imageComponent.sprite = hoverSprite;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (imageComponent != null)
        {
            imageComponent.sprite = defaultSprite;
        }
    }
}
