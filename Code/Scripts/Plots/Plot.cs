using System.Collections.Generic;
using UnityEngine;

public class Plot : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] public bool constructable; //Flage to indicate if a plot is construblae or not (oeg. other structure already buidl)

    private GameObject placedStructure; // Used to access new structure we build
    private bool anyPlotNotConstructable = false; // Flag to check if structures larger then 1x1 have some unconstructable plots
    private bool isDragging = false;

    private static List<SpriteRenderer> plotsToColor = new List<SpriteRenderer>();

    private AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindWithTag("Audio").GetComponent<AudioManager>();
    }   

    private void OnMouseEnter()
    {
        Structure structureToBuild = BuildManager.main.GetSelectedStructure();
        if (structureToBuild == null) return;

        //If a structure is selected we show it over the plot with a 0.5 opacity and offest based on it's size
        Vector3 instantiationPosition = transform.position + GridManager.Instance.CalculateStructureOffsetPosition(structureToBuild.size[0], structureToBuild.size[1]);
        BuildManager.main.UpdatePreviewPosition(instantiationPosition);

        // We set the opacity of the entire structure to 50% and relvant color based on if constructable
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

        // Redo the On Mouse enter test for iphone bug
        //If a structure is selected we show it over the plot with a 0.5 opacity and offest based on it's size
        Vector3 instantiationPosition = transform.position + GridManager.Instance.CalculateStructureOffsetPosition(structureToBuild.size[0], structureToBuild.size[1]);
        BuildManager.main.UpdatePreviewPosition(instantiationPosition);

        // We set the opacity of the entire structure to 50% and relvant color based on if constructable
        CheckPlotConstructability(structureToBuild);

        if (anyPlotNotConstructable)
        {
            BuildManager.main.DeselectStructure();
            audioManager.PlaySFX(audioManager.badActionSFX);
            return;
        }

        PlaceStructureIfPossible();
    }

    // Function to check that all the plots under the structure to build size are not
    // occupied and not out of bounds
    private void CheckPlotConstructability(Structure structureToBuild)
    {
        Vector2Int gridPosition = GridManager.Instance.WorldToGridCoordinates(transform.position);
        anyPlotNotConstructable = false;

        var size = new int[] {structureToBuild.size[0], structureToBuild.size[1]};
        
        for (int x = 0; x < size[0]; x++)
        {
            for (int y = 0; y < size[1]; y++)
            {
                int checkX = gridPosition.x + x;
                int checkY = gridPosition.y + y;

                // First we check that all plots are constructable and raise flag if one is not
                bool isPlotConstructable = GridManager.Instance.IsPlotConstructable(checkX, checkY);
                if (!isPlotConstructable) anyPlotNotConstructable = true;
                
                // Then color each plot red or green for visual feedback
                SpriteRenderer plotSr = GridManager.Instance.GetPlotSpriteRenderer(checkX, checkY);
                if (plotSr != null) // check so we don't try to color out of bounds plots
                {
                    plotsToColor.Add(plotSr);
                    plotSr.color = isPlotConstructable ? Color.green : Color.red;        
                }
            }
        }

        // Finally we set the opacity of the entire structure to 50% and relevant color based on flag
        if (anyPlotNotConstructable)
        {
            BuildManager.main.SetOpacity(BuildManager.main.structurePreviewInstance, 0.5f, Color.red);
        }else {
            BuildManager.main.SetOpacity(BuildManager.main.structurePreviewInstance, 0.5f, Color.green);
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


            // Then set the opacity (base only) to 1
            Transform BaseTransform = placedStructure.transform.Find("Base");
            if (BaseTransform != null)
            {
                GameObject Base = BaseTransform.gameObject; 
                BuildManager.main.SetOpacity(Base, 1f, Color.white);
            }

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
