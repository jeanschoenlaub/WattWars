using UnityEngine;

[CreateAssetMenu(fileName = "New Building", menuName = "IdleBuilding")]
public class IdleBuilding : ScriptableObject
{
    public float power;
    public int cost;
    public float timeToBuild;
    public Sprite onSprite;
    public Sprite offSprite;
}
