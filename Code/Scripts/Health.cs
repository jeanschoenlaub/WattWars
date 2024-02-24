using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] private int healthPoint = 2;

    private bool isDestroyed = false;

    public void TakeDamage(int dmg){
        healthPoint -= dmg;

        if (healthPoint <= 0 && !isDestroyed){
            EnemySpawner.onEnemyDestroy.Invoke();
            Destroy(gameObject);
            isDestroyed = true;
        }
    }
}
