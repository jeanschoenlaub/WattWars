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

    private Plot[,] gridPlots;  // plotsSorted in awake  

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

    void InitializePlotReferences()
    {
        gridPlots = new Plot[width, height];

        foreach (Plot plot in plots)
        {
            Vector2Int gridPos = WorldToGridCoordinates(plot.transform.position);
            if (gridPos.x >= 0 && gridPos.x < width && gridPos.y >= 0 && gridPos.y < height)
            {
                gridPlots[gridPos.x, gridPos.y] = plot;
            }
        }
    }

    public SpriteRenderer GetPlotSpriteRenderer(int x, int y)
    {
        if (x >= 0 && x < width && y >= 0 && y < height && gridPlots[x, y] != null)
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
        if (x < 0 || x >= width || y < 0 || y >= height) return false; // Out of bounds
        return gridPlots[x, y].constructable; // Return true if not occupied
    }

    public Vector2Int WorldToGridCoordinates(Vector3 worldPosition)
    {
        //Not sure on why +2 and +1 just hard tested with current set-up
        int x = Mathf.FloorToInt((worldPosition.x - gridOrigin.x) / plotSize)+2;
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
                if (x < width && y < height)
                {
                    gridPlots[x, y].constructable = false; // Mark as occupied
                }
            }
        }
    }

    
}