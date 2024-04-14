using UnityEngine;

[CreateAssetMenu(fileName = "New Tower", menuName = "Structure/Tower")]
public class Tower : Structure
{
    public GameObject rangeCircle;
    public float targetingRange;
    public float bulletPerSeconds;
}
