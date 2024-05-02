using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    [Header("References")]
    [SerializeField] public Vector2 gridOrigin = new Vector2(); // World position of the grid's origin (bottom-left corner)
    [SerializeField] public float plotSize; // The size of a plot in world units
    [SerializeField] public int width ; // The number of plot horizontal
    [SerializeField] public int height ; // The number of plot horizontal
    [SerializeField] private Plot[] plots; // dump of all plots

    [SerializeField] public Plot[,] gridPlots;  // plots sorted in awake  

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        InitializePlotReferences(); // Call this after initializing the grid
    }

    public void PlaceStructureAtPosition(Vector3 worldPosition, Structure structure)
    {
        Vector2Int gridPos = WorldToGridCoordinates(worldPosition);
        if (gridPos.x >= 0 && gridPos.x <= width && gridPos.y >= 0 && gridPos.y <= height)
        {
            Plot targetPlot = gridPlots[gridPos.x, gridPos.y];
            if (targetPlot != null && targetPlot.constructable)
            {
                 targetPlot.HandleConstruction();
            }
            else{Debug.Log($"Plot at ({gridPos.x}, {gridPos.y}) is not constructable.");}
        }
        else{Debug.Log("Pointer up position is out of grid bounds.");}
    }
    

    void InitializePlotReferences()
    {
        gridPlots = new Plot[width+1, height+1];

        foreach (Plot plot in plots)
        {
            Vector2Int gridPos = WorldToGridCoordinates(plot.transform.position);
            if (gridPos.x >= 0 && gridPos.x <= width && gridPos.y >= 0 && gridPos.y <= height)
            {
                gridPlots[gridPos.x, gridPos.y] = plot;
            }
            else{
                // Raise error if the level manager width, height or plots is badly initialised
                Debug.Log("problem with initialising Plot references - Check GridManager Script");
            }
        }
    }

    public SpriteRenderer GetPlotSpriteRenderer(int x, int y)
    {
        if (x >= 0 && x <= width && y >= 0 && y <= height && gridPlots[x, y] != null)
        {
            return gridPlots[x, y].GetComponent<SpriteRenderer>();
        }
        return null;
    }

    public Vector3 CalculateStructureOffsetPosition(int structureSizeX, int structureSizeY){
        Vector3 offset = new Vector3((structureSizeX - 1) *  plotSize/2, (structureSizeY - 1) *  plotSize/2, 0);
        return offset;
    }

    // Check if a specific plot is available
    public bool IsPlotConstructable(int x, int y)
    {
        Debug.Log(x);
        Debug.Log(y);
        if (x < 0 || x > width || y < 0 || y > height) {
            return false; // Out of bounds
        }
        return gridPlots[x, y].constructable; // Return true if not occupied
    }

    public Vector2Int WorldToGridCoordinates(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt((worldPosition.x - gridOrigin.x) / plotSize)+1;
        int y = Mathf.FloorToInt((worldPosition.y - gridOrigin.y) / plotSize)+1;
        return new Vector2Int(x, y);
    }

    // Reserve plots for a tower
    public void ReservePlots(int startX, int startY, int sizeX, int sizeY)
    {
        for (int x = startX; x < startX + sizeX; x++)
        {
            for (int y = startY; y < startY + sizeY; y++)
            {
                // Check if x and y are within the bounds of the gridPlots array
                if (x >= 0 && x < gridPlots.GetLength(0) && y >= 0 && y < gridPlots.GetLength(1))
                {
                    // It's now safe to access gridPlots[x, y]
                    if (gridPlots[x, y] != null)
                    {
                        gridPlots[x, y].constructable = false; // Mark as occupied
                    }
                    else
                    {
                        Debug.Log("Error reserving plot, check GridManager logic");
                    }
                }
                else
                {
                    Debug.Log("Attempted to access gridPlots[{x}, {y}], which is out of bounds. Check GridManager logic");
                }
            }
        }
    }

    
}