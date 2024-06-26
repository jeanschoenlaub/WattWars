using System.Collections.Generic;
using UnityEngine;

// Each constructibe square in the game get's assigned a plot script
public class Plot : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer sr; // The plots sprite to color green and red to show wehre player can build
    [SerializeField] public bool constructable; //Flag to indicate if a plot is constructable or not (oeg. other structure already build)
    [SerializeField] public bool buildingOnly; //Flag to indicate if a plot is constructable or not (oeg. other structure already build)


    private GameObject placedStructure; // Used to access new structure we build
    private bool anyPlotNotConstructable = false; // Flag to check if structures larger then 1x1 have some unconstructable plots
    private static List<SpriteRenderer> plotsToColor = new List<SpriteRenderer>(); // We sometimes need to color multiple plots and decolor them from one place

    private AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindWithTag("Audio").GetComponent<AudioManager>();
    }   

    //When dragging structures we display previews of tower over the plot we are dragging over
    private void OnMouseEnter()
    {
        Structure structureToBuild = BuildManager.main.GetSelectedStructure();
        if (structureToBuild == null) return;

        //If a structure is selected we show it over the plot with a 0.5 opacity and offest based on it's size
        Vector3 instantiationPosition = transform.position + GridManager.Instance.CalculateStructureOffsetPosition(structureToBuild.size[0], structureToBuild.size[1]);
        BuildManager.main.UpdatePreviewPosition(instantiationPosition);

        // Check if any plot is occupied or off grid and color plot red if so
        // If it is a Building we also need to color based on if plot allows building
        anyPlotNotConstructable = CheckPlotConstructability(structureToBuild);

        // Finally we set the opacity of the entire structure to 50% and relevant color based on flag
        if (anyPlotNotConstructable)
        {
            BuildManager.main.SetOpacity(BuildManager.main.structurePreviewInstance, 0.5f, Color.red);
        }else {
            BuildManager.main.SetOpacity(BuildManager.main.structurePreviewInstance, 0.5f, Color.green);
        }
    }

    private void OnMouseExit()
    {
        RemovePlotsColor();
    }

    private void RemovePlotsColor(){
        foreach (var plotSr in plotsToColor)
        {
            plotSr.color = Color.white;
        }
        plotsToColor.Clear();
    }

    public void ClearStructurePreview(){
        BuildManager.main.DeselectStructure();
        RemovePlotsColor();
    }

    private void OnMouseDown()
    {
        HandleConstruction(); 
    }

    // If the structure is in a constructable spot and the player has enough moeny --> Build, else clear previewx
    public void HandleConstruction(){
        var structureToBuild = BuildManager.main.GetSelectedStructure();
        if (structureToBuild == null) return;

        if (!anyPlotNotConstructable && constructable==true && LevelManager.main.SpendCurrency(structureToBuild.cost))
        {
            PlaceStructure(structureToBuild);
        }else{
            audioManager.PlaySFX(audioManager.badActionSFX);
            ClearStructurePreview();
            return;
        }
    }

    // Function to check that all the plots under the structure to build size are not occupied and not out of bounds
    // colors each plot accordingly and return true if any plot not constructable 
    private bool CheckPlotConstructability(Structure structureToBuild)
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
                bool isPlotConstructable = false;
                if (structureToBuild is Building) {
                    isPlotConstructable = GridManager.Instance.IsPlotBuildingConstructable(checkX, checkY);
                }
                else{
                   isPlotConstructable = GridManager.Instance.IsPlotConstructable(checkX, checkY);
                }
                
                if (!isPlotConstructable) {anyPlotNotConstructable = true;}
                
                // Then color each plot red or green for visual feedback
                SpriteRenderer plotSr = GridManager.Instance.GetPlotSpriteRenderer(checkX, checkY);
                if (plotSr != null) // check so we don't try to color out of bounds plots
                {
                    plotsToColor.Add(plotSr);
                    plotSr.color = isPlotConstructable ? Color.green : Color.red;        
                }
            }
        }

        return anyPlotNotConstructable;
    }

    private void PlaceStructure(Structure structureToBuild)
    {
        GameObject prefab = structureToBuild.prefab; // Declare prefab as GameObject
        int [] size = structureToBuild.size;

        // If prefab
        if (prefab != null)
        {
            // First we calculate the actual position which is the center of current plot +  offet based on structure size
            Vector3 instantiationPosition = transform.position + GridManager.Instance.CalculateStructureOffsetPosition(structureToBuild.size[0], structureToBuild.size[1]);
            
            // Then we place structure, play sound
            placedStructure = Instantiate(prefab, instantiationPosition , Quaternion.identity);
            audioManager.PlaySFX(audioManager.buildSFX);

            // Then set the opacity (base only) to 1
            Transform BaseTransform = placedStructure.transform.Find("Base");
            if (BaseTransform != null)
            {
                GameObject Base = BaseTransform.gameObject; 
                BuildManager.main.SetOpacity(Base, 1f, Color.white);
            }
            
            // Make sure a building tag is assigned to building structures
            if (structureToBuild is Building) { 
                placedStructure.tag = "Building";
            }

            //Finally we mark the plot(s) as reserved by this stuct so can't build others on top
            Vector2Int gridPosition = GridManager.Instance.WorldToGridCoordinates(transform.position);
            GridManager.Instance.ReservePlots(gridPosition[0],gridPosition[1],size[0],size[1]);

            //Trigger Cooldownand clear preview
            ShopManager.main.StartCooldown(structureToBuild);
            ClearStructurePreview();
        }else {
            Debug.Log("Error fetcing structure prefab, make sure to assign in inspector");
        }
    }
}
