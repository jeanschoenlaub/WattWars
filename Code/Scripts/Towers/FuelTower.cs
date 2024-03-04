using UnityEngine;

public class FuelTower : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private GameObject fuelPrefab;
    [SerializeField] private Transform firingPoint;
    [SerializeField] private LayerMask enemyMask;

    [Header("Attribute")]
    [SerializeField] private float targetingRange = 5f;
    [SerializeField] private float fuelPerSeconds = 1f;

    private Transform target;
    private float timeUntilFire;

    private void Update(){
        if (target == null){
            FindFuelTarget();
            return;
        } else {
            float distance = Vector2.Distance(transform.position, target.position);
            if (distance > targetingRange){
                target = null;
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
        fuelScript.SetTarget(target);
    }

    private void FindFuelTarget()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, targetingRange - 0.2f, Vector2.zero, 0f, enemyMask);
        if (hits.Length > 0)
        {
            Transform furthestTarget = null;
            float maxProgress = -1; // Start with a progress lower than any enemy could have

            foreach (RaycastHit2D hit in hits)
            {
                EnemyMovement enemyMovement = hit.transform.GetComponent<EnemyMovement>();
                Health enemyHealth = hit.transform.GetComponent<Health>(); // Assuming 'Health' holds the 'fuelLives' attribute
                
                // Check both enemyMovement and enemyHealth for null to avoid NullReferenceException
                if (enemyMovement != null && enemyHealth != null && enemyHealth.fuelLives != 0)
                {
                    if (enemyMovement.pathProgress > maxProgress)
                    {
                        furthestTarget = hit.transform;
                        maxProgress = enemyMovement.pathProgress;
                    }
                }
            }
            target = furthestTarget; // Set the found target only if fuelLives is not zero
        }
    }
}
