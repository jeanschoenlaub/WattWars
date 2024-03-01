using UnityEngine;

public class Turret : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject electronPrefab;
    [SerializeField] private Transform firingPoint;
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private LayerMask buildingMask;

    [Header("Attribute")]
    [SerializeField] private float targetingRange = 5f;
    [SerializeField] private float electronPerSeconds = 1f;

    private Transform target;
    private float timeUntilFire;

    private void Update(){
 
        FindTarget();

        if (target){
            int currentGameSpeed = LevelManager.GetGameSpeed();
            timeUntilFire += Time.deltaTime*currentGameSpeed;

            if (timeUntilFire >= 1f/ electronPerSeconds){
                Shoot(); 
                timeUntilFire = 0f;
            }
        }
    }

    private void Shoot(){
        GameObject electronObj = Instantiate(electronPrefab, firingPoint.position, Quaternion.identity);
        Bullet electronScript = electronObj.GetComponent<Bullet>();
        electronScript.SetTarget(target);
    }

    private void FindTarget()
    {
        target = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        // Find the closest enemy
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Vector3 directionToTarget = enemy.transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                EnemyMovement enemyMovement = enemy.GetComponent<EnemyMovement>();
                Health enemyHealth = enemy.GetComponent<Health>();
                // Check if enemy can be targeted based on health and is within range
                if (enemyHealth != null && enemyHealth.elecLives != 0 && dSqrToTarget <= (targetingRange * targetingRange))
                {
                    closestDistanceSqr = dSqrToTarget;
                    target = enemy.transform;
                }
            }
        }

        // If no enemy is targeted, find the closest building
        if (target == null)
        {
            closestDistanceSqr = Mathf.Infinity; // Reset for building search
            foreach (GameObject building in GameObject.FindGameObjectsWithTag("Building"))
            {
                Vector3 directionToTarget = building.transform.position - currentPosition;
                float dSqrToTarget = directionToTarget.sqrMagnitude;
                if (dSqrToTarget < closestDistanceSqr && dSqrToTarget <= (targetingRange * targetingRange))
                {
                    closestDistanceSqr = dSqrToTarget;
                    target = building.transform;
                }
            }
        }
    }
}
