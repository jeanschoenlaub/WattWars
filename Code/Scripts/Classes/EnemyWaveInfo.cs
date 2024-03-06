using UnityEngine;

[System.Serializable]
public class EnemyWaveInfo {
    public GameObject enemyPrefab; // Reference to the enemy prefab
    public int quantity; // Quantity of this enemy type in the wave
    public float spawnDelay; // Delay before starting to spawn this enemy type
    public float spawnInterval; // Time interval between spawns of this enemy type
}