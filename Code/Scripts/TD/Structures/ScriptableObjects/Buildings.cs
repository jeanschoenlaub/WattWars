using UnityEngine;

[CreateAssetMenu(fileName = "New Building", menuName = "Structure/Building")]
public class Building : Structure
{
    public BulletType bulletType;
    public float energyRequired; // Energy required to make money
    public int moneyGenerated;
}
