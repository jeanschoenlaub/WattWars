using UnityEngine;
using UnityEngine.EventSystems; 
using UnityEngine.UI; 

// Make sure your class implements the required interfaces for pointer events
public class HoverShaderSwitchUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Sprite defaultSprite;
    public Sprite hoverSprite; 

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
