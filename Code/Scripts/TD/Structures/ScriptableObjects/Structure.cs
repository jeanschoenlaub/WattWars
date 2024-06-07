using UnityEngine;

[CreateAssetMenu(fileName = "New Structure", menuName = "Structure/Base")]
public abstract class Structure : ScriptableObject
{
    public string structureName;
    public bool isBuilding;
    public int cost;
    public int[] size; // To-Do switch to vector2int
    public GameObject prefab;
    public float buildCooldown;  // Cooldown time in seconds for building this structure
}

