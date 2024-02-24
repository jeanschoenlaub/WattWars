using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemySpawner : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private GameObject[] enemyPrefabs;


    [Header("Attributes")]
    [SerializeField] private int baseEnemyNb = 8;
    [SerializeField] private float enemiesPerSeconds = 0.5f;
    [SerializeField] private float timeBetweenWaves = 5f;

    [SerializeField] private float enemyIncreasePerWave = 0.7f;


    [Header("Events")]
    public static UnityEvent onEnemyDestroy = new UnityEvent();
    
    private int currentWave = 1;
    private float timeSinceLastSpawn;

    private int enemiesLeftToSpawn;
    private int enemiesAlive;
    private bool isSpawning = false;

    private void Awake(){
        onEnemyDestroy.AddListener(EnemyDestroyed);
    }

    private void Start(){
        StartCoroutine(StartWave());
    }

    private void Update(){
        if (!isSpawning){return;}

        timeSinceLastSpawn += Time.deltaTime; 

        if (timeSinceLastSpawn >= (1f/enemiesPerSeconds) && enemiesLeftToSpawn > 0){
            SpawnEnemy();
            enemiesLeftToSpawn--;
            enemiesAlive++;
            timeSinceLastSpawn=0f;
        }

        if (enemiesAlive == 0 && enemiesLeftToSpawn ==0){
            EndWave();
        }
    }

    private void EndWave(){

        isSpawning = false;
        timeSinceLastSpawn = 0f;
        currentWave++;

        StartCoroutine(StartWave());
    }

    private void EnemyDestroyed(){;
        enemiesAlive--;
    }

    private void SpawnEnemy(){
        GameObject prefabToSpawn = enemyPrefabs[0];
        Instantiate(prefabToSpawn, LevelManager.main.startPoint.position, Quaternion.identity);
    }
    

    private IEnumerator StartWave(){
        yield return new WaitForSeconds(timeBetweenWaves);

        isSpawning = true;
        enemiesLeftToSpawn = EnemiesPerWave();
    }

    private int EnemiesPerWave(){
        return Mathf.RoundToInt(baseEnemyNb * Mathf.Pow(currentWave, enemyIncreasePerWave));
    }
}
