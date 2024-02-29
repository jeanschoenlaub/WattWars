using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager main;

    [Header("References")]
    [SerializeField] private Tower[] towers;
    [SerializeField] private Building[] buildings; // Add an array for buildings

    private object selectedStructure; // object to store either Tower or Building
    public GameObject structurePreviewInstance; // Generalized name for preview instance


    void Awake()
    {
        main = this;
    }

    public object GetSelectedStructure()
    {
        return selectedStructure;
    }

    public void DeselectStructure()
    {
        selectedStructure=null;
    }

    // public void SetSelectedTower(int towerIndex)
    // {
    //     // Destroy existing preview if switching towers
    //     if (structurePreviewInstance != null)
    //     {
    //       Destroy(towerPreviewInstance);
    //     }

    //     selectedTower = towers[towerIndex];
    //     // Create a new preview instance initially transparent and script disabled
    //     towerPreviewInstance = Instantiate(selectedTower.prefab);
    //     SetOpacity(towerPreviewInstance,0f,Color.white);
    //     DisableComponents(towerPreviewInstance);
    // }

    // Method to select a tower, keeping your existing logic
    public void SetSelectedTower(int towerIndex)
    {
        ClearStructurePreview();
        selectedStructure = towers[towerIndex];
        CreateStructurePreview(((Tower)selectedStructure).prefab);
    }

    // Add a method to select a building
    public void SetSelectedBuilding(int buildingIndex)
    {
        ClearStructurePreview();
        selectedStructure = buildings[buildingIndex];
        CreateStructurePreview(((Building)selectedStructure).prefab);
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
    private void CreateStructurePreview(GameObject prefab)
    {
        structurePreviewInstance = Instantiate(prefab);
        SetOpacity(structurePreviewInstance, 0.5f, Color.white);
        DisableComponents(structurePreviewInstance);
    }

    // Utility method to set opacity
    public void SetOpacity(GameObject obj, float opacity, Color color)
    {
        foreach (var renderer in obj.GetComponentsInChildren<SpriteRenderer>())
        {
            color.a = opacity;
            renderer.color = color;
        }
    }

    // Utility method to disable components
    private void DisableComponents(GameObject obj)
    {
        foreach (var collider in obj.GetComponentsInChildren<Collider2D>()){
            collider.enabled = false;
        }
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