using UnityEngine;

public class FuelTower : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private GameObject fuelPrefab;
    [SerializeField] private Transform firingPoint;
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private GameObject rangeCircle;

    [Header("Attribute")]
    [SerializeField] private float targetingRange = 5f;
    [SerializeField] private float fuelPerSeconds = 1f;

    private Transform furthestTarget;
    private float timeUntilFire;

    private void Update(){
        if (furthestTarget == null){
            FindFuelTarget();
            return;
        } else {
            float distance = Vector2.Distance(transform.position, furthestTarget.position);
            if (distance > targetingRange){
                furthestTarget = null;
                return; 
            }
        }


        int currentGameSpeed = LevelManager.GetGameSpeed();
        timeUntilFire += Time.deltaTime*currentGameSpeed;

        if (timeUntilFire >= 1f/ fuelPerSeconds){
            Shoot(); 
            timeUntilFire = 0f;
        }
    }
    private void Shoot(){
        GameObject fuelObj = Instantiate(fuelPrefab, firingPoint.position, Quaternion.identity);
        Bullet fuelScript = fuelObj.GetComponent<Bullet>();
        fuelScript.SetTarget(furthestTarget);
    }

    private void FindFuelTarget()
    {
        furthestTarget = null;
        float closestDistance = Mathf.Infinity;
        float maxProgress = -1;
    
        // Find the closest enemy
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            // Used to determine the furthest ennemy on the path
            float enemyProgress = enemy.GetComponent<EnemyMovement>().pathProgress;
             // Start with a progress lower than any enemy could have

            // Used to determine if enemy is in range
            Vector3 directionToTarget = enemy.transform.position -  transform.position;
            float dSqrToTarget = directionToTarget.sqrMagnitude;

            if (enemyProgress < closestDistance)
            {
                
                EnemyMovement enemyMovement = enemy.GetComponent<EnemyMovement>();
                

                Health enemyHealth = enemy.GetComponent<Health>();
                // Check if enemy can be targeted based on health and is within range
                if (enemyHealth != null && enemyHealth.fuelLives != 0 && dSqrToTarget <= (targetingRange * targetingRange))
                {
                   if (enemyMovement.pathProgress > maxProgress)
                    {
                        maxProgress = enemyMovement.pathProgress;
                        furthestTarget = enemy.transform;
                       
                    }
                }
            }
        }
    }
}
