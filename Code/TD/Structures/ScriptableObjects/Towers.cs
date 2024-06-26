using UnityEngine;

[CreateAssetMenu(fileName = "New Tower", menuName = "Structure/Tower")]
public class Tower : Structure
{
    public GameObject rangeCircle;
    public float targetingRange;
    public float bulletPerSeconds;
    public BulletType bulletType;
    public float bulletDamage;
    public float maxCharge; // For batteries only
}
