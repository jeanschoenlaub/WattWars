using System.Collections.Generic;
using UnityEngine;

public class Plot : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] public bool constructable;

    private GameObject placedStructure; // Generalized from plotTower
    private bool anyPlotNotConstructable = false;

    private static List<SpriteRenderer> plotsToColor = new List<SpriteRenderer>();

    private void OnMouseEnter()
    {
        // Generalize this to work with any selected structure, not just towers
        var structureToBuild = BuildManager.main.GetSelectedStructure();
        if (structureToBuild == null) return;

        //BuildManager.main.DisableComponents(BuildManager.main.structurePreviewInstance);
        BuildManager.main.UpdatePreviewPosition(transform.position);
        BuildManager.main.SetOpacity(BuildManager.main.structurePreviewInstance, 0.5f, Color.white);

        CheckPlotConstructability(structureToBuild);
    }

    private void OnMouseExit()
    {
        foreach (var plotSr in plotsToColor)
        {
            plotSr.color = Color.white;
        }
        plotsToColor.Clear();
    }

    private void OnMouseDown()
    {
        if (placedStructure != null || anyPlotNotConstructable)
        {
            BuildManager.main.DeselectStructure();
            return;
        }

        PlaceStructureIfPossible();
    }

    private void CheckPlotConstructability(object structureToBuild)
    {
        Vector2Int gridPosition = GridManager.Instance.WorldToGridCoordinates(transform.position);
        anyPlotNotConstructable = false;

        var size = new int[] { 0, 0 };

        if (structureToBuild is Tower tower)
        {
            size = new int[] {tower.size[0], tower.size[1]};
        }
        else if (structureToBuild is Building building)
        {
            size = new int[] {building.size[0], building.size[1]};
        }

        for (int x = 0; x < size[0]; x++)
        {
            for (int y = 0; y < size[1]; y++)
            {
                int checkX = gridPosition.x + x;
                int checkY = gridPosition.y + y;
                SpriteRenderer plotSr = GridManager.Instance.GetPlotSpriteRenderer(checkX, checkY);
                if (plotSr != null)
                {
                    plotsToColor.Add(plotSr);
                    bool isPlotConstructable = GridManager.Instance.IsPlotConstructable(checkX, checkY);
                    plotSr.color = isPlotConstructable ? Color.green : Color.red;

                    if (!isPlotConstructable) anyPlotNotConstructable = true;
                }
            }
        }

        if (anyPlotNotConstructable)
        {
            BuildManager.main.SetOpacity(BuildManager.main.structurePreviewInstance, 0.5f, Color.red);
        }
    }

    private void PlaceStructureIfPossible()
    {
       var structureToBuild = BuildManager.main.GetSelectedStructure();

        int cost = 0;
        GameObject prefab = null; // Declare prefab as GameObject

        if (structureToBuild is Tower tower)
        {
            cost = tower.cost;
            prefab = tower.prefab; // Assigning the prefab from the Tower
        }
        else if (structureToBuild is Building building)
        {
            cost = building.cost;
            prefab = building.prefab; // Assigning the prefab from the Building
        }

        // Ensure prefab is not null to avoid errors in Instantiate
        if (prefab != null && LevelManager.main.SpendCurrency(cost))
        {
            placedStructure = Instantiate(prefab, transform.position, Quaternion.identity);
            if (structureToBuild is Building ) { placedStructure.tag = "Building";}
            BuildManager.main.SetOpacity(placedStructure, 1f, Color.white);
            BuildManager.main.DeselectStructure();
        }
    }
}
