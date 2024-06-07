using UnityEngine;

[CreateAssetMenu(fileName = "BuildingDatabase", menuName = "IdleGame/BuildingDatabase")]
public class BuildingDatabase : ScriptableObject
{
    public IdleBuilding[] buildings;
    
    public IdleBuilding GetBuildingByName(string name)
    {
        foreach (var building in buildings)
        {
            if (building.buildingName == name)
                return building;
        }
        return null; // Return null if no building matches the name
    }
}

[CreateAssetMenu(fileName = "New Building", menuName = "IdleGame/IdleBuilding")]
public class IdleBuilding : ScriptableObject
{
    public string buildingName;
    public float power;
    public int cost;
    public float timeToBuild;
    public Sprite onSprite;
    public Sprite offSprite;
}
