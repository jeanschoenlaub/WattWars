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

        //If a structure is selected we show it with a 0.5 opacity and offest based on it's size
        Vector3 instantiationPosition = transform.position + GridManager.Instance.CalculateStructureOffsetPosition(structureToBuild.size[0], structureToBuild.size[1]);
        BuildManager.main.UpdatePreviewPosition(instantiationPosition);
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
        var structureToBuild = BuildManager.main.GetSelectedStructure();
        if (structureToBuild == null) return;
        
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

        // If prefab and player has enough money then we build
        if (prefab != null && LevelManager.main.SpendCurrency(structureToBuild.cost))
        {
            // First we calculate the actual position which is the center of current plot +  offet based on structure size
            Vector3 instantiationPosition = transform.position + GridManager.Instance.CalculateStructureOffsetPosition(structureToBuild.size[0], structureToBuild.size[1]);
            
            // Then we place structure, play sound, change from a transparent preview opac and deselect struct
            placedStructure = Instantiate(prefab, instantiationPosition , Quaternion.identity);
            audioManager.PlaySFX(audioManager.buildSFX);
            BuildManager.main.SetOpacity(placedStructure, 1f, Color.white);
            BuildManager.main.DeselectStructure();
            if (structureToBuild is Building) { 
                placedStructure.tag = "Building";
            }

            //Finally we mark the plot(s) as reserved by this stuct so can't build others on top
            Vector2Int gridPosition = GridManager.Instance.WorldToGridCoordinates(transform.position);
            GridManager.Instance.ReservePlots(gridPosition[0],gridPosition[1],size[0],size[1]);
           
        }else {
            // If not enough money we deselect and play error sound
            BuildManager.main.DeselectStructure();
            audioManager.PlaySFX(audioManager.badActionSFX);
        }
    }
}
