using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlotScript : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Color hoverColor;

    private GameObject tower; //no build tower by defaut
    private Color startColor;

    private void Start(){
        startColor = sr.color; 
    }

    private void OnMouseEnter (){ 
        sr.color = hoverColor;
    }

    private void OnMouseExit(){
        sr.color = startColor;
    }

    private void OnMouseDown(){
        if ( tower != null) { return;}

        Tower towerToBuild = BuildManager.main.GetSelectedTower();

        //Level manager handles if not enough money
        if ( LevelManager.main.SpendCurrency(towerToBuild.cost) ){ 
            tower = Instantiate(towerToBuild.prefab, transform.position, Quaternion.identity);
        }
        
    }

}
