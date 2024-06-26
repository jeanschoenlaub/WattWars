using UnityEngine;

[CreateAssetMenu(fileName = "New Structure", menuName = "Structure/Base")]
public abstract class Structure : ScriptableObject
{
    public string structureName;
    public bool isBuilding;
    public int initialCost;
    public int currentCost;
    public int[] size; // To-Do switch to vector2int
    public GameObject prefab;
    public Sprite icon;
    public float buildCooldown;  // Cooldown time in seconds for building this structure

    // Method to initialize costs
    public virtual void InitializeCosts()
    {
        currentCost = initialCost;
    }

    // Method to upgrade structure with specified cost increment
    public virtual void UpdateCost(int newCost)
    {
        Debug.Log("updatiing cost to"+ newCost);
        currentCost = newCost;
    }
}
