using System.Collections.Generic;
using UnityEngine;

public class Plot : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] public bool constructable; // Can't build on some tile prefabs like roads

    private GameObject plotTower; // Tower object on each plot, default no tower
    private bool anyPlotNotConstructable = false; //lag to track if any plots (for 2x2 towers) not constr.

    // Static list to track colored plots for made attribute to set and reset from different functions
    private static List<SpriteRenderer> plotsToColor = new List<SpriteRenderer>();

    private void OnMouseEnter()
    {
        // Web some logic if the mouse enters a plot but only if a tower is selected    
        Tower towerToBuild = BuildManager.main.GetSelectedTower();
        if (towerToBuild == null){return;} 

        // First we display a tower at 50% opposity on the selected plot
        BuildManager.main.UpdatePreviewPosition(transform.position);
        BuildManager.main.SetOpacity(BuildManager.main.towerPreviewInstance, 0.5f, Color.white);
        
        //Then we color each plot in green if you can construct a tower on it and red if not
        //And we use GridManager utility functions to know neightbors of plot and tower size
        Vector2Int gridPosition = GridManager.Instance.WorldToGridCoordinates(transform.position);
        anyPlotNotConstructable = false;

        for (int x = 0; x < towerToBuild.size[0]; x++)
        {
            for (int y = 0; y < towerToBuild.size[1]; y++)
            {
                int checkX = gridPosition.x + x;
                int checkY = gridPosition.y + y;
                SpriteRenderer plotSr = GridManager.Instance.GetPlotSpriteRenderer(checkX, checkY);
                if (plotSr != null)
                {
                    plotsToColor.Add(plotSr);
                    bool isPlotConstructable = GridManager.Instance.IsPlotConstructable(checkX, checkY);
                    plotSr.color = isPlotConstructable ? Color.green : Color.red;

                    if (!isPlotConstructable){ anyPlotNotConstructable = true;}
                }
            }
        }

        //If any plots not available, color the entire tower red
        if (anyPlotNotConstructable){
            BuildManager.main.SetOpacity(BuildManager.main.towerPreviewInstance, 0.5f, Color.red);
        }
    }

    private void OnMouseExit(){
        // On MouseExit we reset the color 
        foreach (var plotSr in plotsToColor)
        {
            plotSr.color = Color.white;
        }
        // Clear the list after resetting the colors
        plotsToColor.Clear();
    }

    private void OnMouseDown(){

        // If we can't build because not constructable or there already is a tower we just deselect towerToBuild 
        if (plotTower != null || anyPlotNotConstructable) 
        {
            BuildManager.main.DeselectTower();// Deselect towerToBuild
            return;
        }

        Tower towerToBuild = BuildManager.main.GetSelectedTower();
        if (towerToBuild != null && LevelManager.main.SpendCurrency(towerToBuild.cost))
        {
            plotTower = Instantiate(towerToBuild.prefab, transform.position, Quaternion.identity);
            // Reset tower opacity to 100% upon placing
            BuildManager.main.SetOpacity(plotTower, 1f, Color.white);
            BuildManager.main.DeselectTower();
        }
    }
}
