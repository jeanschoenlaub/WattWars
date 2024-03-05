using UnityEngine;

[CreateAssetMenu(fileName = "New Wave", menuName = "TowerDefense/Wave")]
public class Wave : ScriptableObject {
    public int[] enemies; // Define an EnemyType class or enum as needed
    public int quantity;
    // Additional wave attributes (speed, special effects, etc.)
}