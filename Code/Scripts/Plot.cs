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

    private AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindWithTag("Audio").GetComponent<AudioManager>();
    }   

    

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
            audioManager.PlaySFX(audioManager.badActionSFX);
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
       Structure structureToBuild = BuildManager.main.GetSelectedStructure();

        GameObject prefab = structureToBuild.prefab; // Declare prefab as GameObject
        int [] size = structureToBuild.size;

        // Ensure prefab is not null to avoid errors in Instantiate
        if (prefab != null && LevelManager.main.SpendCurrency(structureToBuild.cost))
        {
            placedStructure = Instantiate(prefab, transform.position, Quaternion.identity);
            Vector2Int gridPosition = GridManager.Instance.WorldToGridCoordinates(transform.position);

            audioManager.PlaySFX(audioManager.buildSFX);
            
            GridManager.Instance.ReservePlots(gridPosition[0],gridPosition[1],size[0],size[1]);

            if (structureToBuild is Building) { 
                placedStructure.tag = "Building";
            }

            BuildManager.main.SetOpacity(placedStructure, 1f, Color.white);
            BuildManager.main.DeselectStructure();
        }else {
            BuildManager.main.SetOpacity(placedStructure, 1f, Color.white);
            BuildManager.main.DeselectStructure();
            audioManager.PlaySFX(audioManager.badActionSFX);
        }
    }
}
