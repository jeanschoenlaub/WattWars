using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlotScript : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Color hoverColor;
    [SerializeField] private bool constructable; // Can't build on some tile prefabs like roads

    private GameObject tower; //no build tower by defaut
    private Color startColor;

    private void Start(){
        startColor = sr.color; 
    }

    private void OnMouseEnter()
    {
        if (BuildManager.main.GetSelectedTower() != null)
        {
            sr.color = hoverColor;
            // Update the position of the tower preview to this plot
            BuildManager.main.UpdatePreviewPosition(transform.position);
            BuildManager.main.SetOpacity(BuildManager.main.towerPreviewInstance, 0.5f, Color.white);
            if (!constructable){
            BuildManager.main.SetOpacity(BuildManager.main.towerPreviewInstance, 0.5f, Color.red);
            }
        }
    }

    private void OnMouseExit(){
        sr.color = startColor;
    }

    private void OnMouseDown(){
        if (tower != null || !constructable) 
        {
            // Deselect tower if clicking on a non-constructable plot or already has a tower
            BuildManager.main.DeselectTower();
            return;
        }

        Tower towerToBuild = BuildManager.main.GetSelectedTower();
        if (towerToBuild != null && LevelManager.main.SpendCurrency(towerToBuild.cost))
        {
            tower = Instantiate(towerToBuild.prefab, transform.position, Quaternion.identity);
            // Reset tower opacity to 100% upon placing
            BuildManager.main.SetOpacity(tower, 1f, Color.white);
            BuildManager.main.DeselectTower();
        }
    }
}
