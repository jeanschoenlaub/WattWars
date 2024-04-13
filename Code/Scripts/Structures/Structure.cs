using UnityEngine;

[CreateAssetMenu(fileName = "New Structure", menuName = "Structure/Base")]
public abstract class Structure : ScriptableObject
{
    public string structureName;
    public int cost;
    public int[] size; // TO-dO change to Vector2Int
    public GameObject prefab;
}

