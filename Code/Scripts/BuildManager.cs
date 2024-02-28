using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager main;

    [Header("References")]
    [SerializeField] private Tower[] towers;

    private Tower selectedTower;
    public GameObject towerPreviewInstance;


    void Awake()
    {
        main = this;
    }

    public Tower GetSelectedTower()
    {
        return selectedTower;
    }

    public void SetSelectedTower(int towerIndex)
    {
        // Destroy existing preview if switching towers
        if (towerPreviewInstance != null)
        {
          Destroy(towerPreviewInstance);
        }

        selectedTower = towers[towerIndex];
        // Create a new preview instance initially transparent and script disabled
        towerPreviewInstance = Instantiate(selectedTower.prefab);
        SetOpacity(towerPreviewInstance,0f,Color.white);
        DisableComponents(towerPreviewInstance);
    }

    public void DeselectTower()
    {
        if (towerPreviewInstance != null)
        {
            Destroy(towerPreviewInstance);
        }
        selectedTower = null;
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
        if (towerPreviewInstance != null)
        {
            towerPreviewInstance.transform.position = position;
        }
    }
}