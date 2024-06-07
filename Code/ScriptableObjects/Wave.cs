using UnityEngine;

[CreateAssetMenu(fileName = "New Wave", menuName = "TowerDefense/Wave")]
public class Wave : ScriptableObject {
    public EnemyWaveInfo[] enemies; // EnemyWaveInfo class contains ennemy prefab, spawn info and qty
    public bool night;
}