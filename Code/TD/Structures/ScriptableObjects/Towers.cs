using UnityEngine;

[CreateAssetMenu(fileName = "New Tower", menuName = "Structure/Tower")]
public class Tower : Structure
{
    public GameObject rangeCircle;
    public float targetingRange;
    public float bulletPerSeconds;
    public BulletType bulletType;
    public float initialBulletDamage;
    public float currentBulletDamage;
    public float maxCharge; // For batteries only

    // Method to initialize costs
    public void InitializeDamage()
    {
        currentBulletDamage = initialBulletDamage;
    }

    // Method to upgrade structure with specified cost increment
    public void UpdateBulletDamage(float newbulletDamage)
    {
        currentBulletDamage = newbulletDamage;
    }
}