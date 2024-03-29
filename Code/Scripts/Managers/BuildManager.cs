using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager main;

    [Header("References")]
    [SerializeField] private Tower[] towers;
    [SerializeField] private Building[] buildings; // Add an array for buildings

    private Structure selectedStructure; // object to store either Tower or Building
    public GameObject structurePreviewInstance; // Generalized name for preview instance

    private AudioManager audioManager;

    private void Awake()
    {
        main = this;
        audioManager = GameObject.FindWithTag("Audio").GetComponent<AudioManager>();
    }   

    public Structure GetSelectedStructure()
    {
        return selectedStructure;
    }

    public void DeselectStructure()
    {
        LevelManager.ResumeGame();
        ClearStructurePreview();
        selectedStructure=null;
    }

    public void SetSelectedTower(int towerIndex)
    {
        ClearStructurePreview();
        selectedStructure = towers[towerIndex];
        CreateStructurePreview(((Tower)selectedStructure).prefab, selectedStructure);

        LevelManager.PauseGame();
        audioManager.PlaySFX(audioManager.buildSelected);
    }

    // Add a method to select a building
    public void SetSelectedBuilding(int buildingIndex)
    {
        ClearStructurePreview();
        selectedStructure = buildings[buildingIndex];
        CreateStructurePreview(((Building)selectedStructure).prefab, selectedStructure);

        LevelManager.PauseGame();
        audioManager.PlaySFX(audioManager.buildSelected);
    }

    // Utility method to clear the existing preview
    private void ClearStructurePreview()
    {
        if (structurePreviewInstance != null)
        {
            Destroy(structurePreviewInstance);
        }
        selectedStructure = null;
    }

    // Utility method to create a preview instance
    private void CreateStructurePreview(GameObject prefab, Structure structure)
    {
        structurePreviewInstance = Instantiate(prefab);
        SetOpacity(structurePreviewInstance, 0.5f, Color.white);
        DisableComponents(structurePreviewInstance);

        // If it's a tower we also preview the range of tower with a circle
        if (structure is Tower tower){
            // Find the RangeCircle GameObject within the instantiated prefab
            Transform rangeCircleTransform = structurePreviewInstance.transform.Find("RangeCircle");
            if (rangeCircleTransform != null)
            {
                GameObject rangeCircle = rangeCircleTransform.gameObject; 
                SetOpacity(rangeCircle,0.5f,Color.green);
                rangeCircle.transform.localScale = new Vector3(tower.range, tower.range, 1);
            }
        }
    }

    // Utility method to set opacity of ALL renders (but can be called with specific game objects)
    public void SetOpacity(GameObject obj, float opacity, Color color)
    {
        foreach (var renderer in obj.GetComponentsInChildren<SpriteRenderer>())
        {
            color.a = opacity;
            renderer.color = color;
        }
    }

    // Utility method to disable components (called from shop click)
    private void DisableComponents(GameObject obj)
    {
        obj.tag = "Preview";
        foreach (var script in obj.GetComponentsInChildren<MonoBehaviour>()){
            script.enabled = false;
        }
    }

    // Call this method to update the position of the preview
    public void UpdatePreviewPosition(Vector3 position)
    {
        if (structurePreviewInstance != null)
        {
            structurePreviewInstance.transform.position = position;
        }
    }
}