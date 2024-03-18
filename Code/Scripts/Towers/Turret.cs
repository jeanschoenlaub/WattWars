using UnityEngine;

public class Turret : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject bulletPrefab; //Either electron or fuel
    [SerializeField] private Transform firingPoint;
    [SerializeField] private GameObject switchOnOff;
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private LayerMask buildingMask;
    [SerializeField] private Animator SwitchButtonAnim;
    [SerializeField] private GameObject rangeCircle;

    [Header("Attribute")]
    [SerializeField] private float targetingRange = 5f;
    [SerializeField] private float bulletPerSeconds = 0f;
    [SerializeField] public bool isSwitcheable = false;
    [SerializeField] public bool isElecTower = false;

    private Transform furthestTarget;
    private float timeUntilFire;
    private bool isSwitchedOn = false; // Starts false but when we click down to build turns On

    private void Update(){
 
        if (!isSwitcheable || (isSwitcheable && isSwitchedOn) ){
            
            FindTarget();

            if (furthestTarget){
                int currentGameSpeed = LevelManager.GetGameSpeed();
                timeUntilFire += Time.deltaTime*currentGameSpeed;

                if (timeUntilFire >= 1f/ bulletPerSeconds){
                    Shoot(); 
                    timeUntilFire = 0f;
                }
            }
        }
    }

    private void Shoot(){
        GameObject bulletObj = Instantiate(bulletPrefab, firingPoint.position, Quaternion.identity);
        Bullet bulletScript = bulletObj.GetComponent<Bullet>();
        bulletScript.SetTarget(furthestTarget);
    }

    private void OnMouseDown()
    {
        if (switchOnOff != null )
        {
            isSwitchedOn = !isSwitchedOn; // Toggle the state
            SwitchButtonAnim.SetBool("TurnSwitchOn", isSwitchedOn);
        }
    }

    private void FindTarget()
    {
        Transform furthestEnemy = FindFurthestEnemyWithinRange();
        if (furthestEnemy != null)
        {
            furthestTarget = furthestEnemy;
        }
        else
        {
            Transform furthestSwitcheableTarget = FindClosestSwitcheableTowerWithinRange();
            
            if (furthestSwitcheableTarget != null)
            {
                furthestTarget = furthestSwitcheableTarget;
            }
            else
            {
                furthestTarget = FindClosestBuildingWithinRange();
            }
        }
    }

    private Transform FindFurthestEnemyWithinRange()
    {
        Transform furthestEnemy = null;
        float maxProgress = -1;

        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            if (IsEnemyTargetableAndInRange(enemy, out float enemyProgress) && enemyProgress > maxProgress)
            {
                maxProgress = enemyProgress;
                furthestEnemy = enemy.transform;
            }
        }

        return furthestEnemy;
    }

    
    // This function determines if an enemy is within targeting range and eligible to be targeted
    // based on the tower's capabilities, specifically targeting enemies with "elecLives" if the
    //  tower has a positive "electronPerSeconds" rate, or those with "fuelLives" for a positive 
    //  "fuelPerSeconds" rate. It uses the enemy's health component and position relative to the 
    //  tower to make this determination, returning true for targetable enemies.
    private bool IsEnemyTargetableAndInRange(GameObject enemy, out float enemyProgress)
    {
        enemyProgress = enemy.GetComponent<EnemyMovement>().pathProgress;
        Vector3 directionToTarget = enemy.transform.position - transform.position;
        float dSqrToTarget = directionToTarget.sqrMagnitude;
        Health enemyHealth = enemy.GetComponent<Health>();

        // Determine if the enemy is within the targeting range
        bool isInRange = dSqrToTarget <= (targetingRange * targetingRange);

        if (enemyHealth == null || !isInRange)
        {
            return false; // Early exit if the enemy health component is missing or if the enemy is out of range
        }

        // Targeting logic based on tower's abilities
        if ( isElecTower && enemyHealth.elecLives > 0)
        {
            // Tower can target enemies with elec lives
            return true;
        }
        else if (!isElecTower && enemyHealth.fuelLives > 0)
        {
            // Tower can target enemies with fuel lives
            return true;
        }

        // If none of the conditions are met, the enemy is not targetable
        return false;
    }

    private Transform FindClosestBuildingWithinRange()
    {
        Transform closestBuilding = null;
        float closestDistanceSqr = Mathf.Infinity;

        foreach (GameObject building in GameObject.FindGameObjectsWithTag("Building"))
        {
            Vector3 directionToBuilding = building.transform.position - transform.position;
            float dSqrToBuilding = directionToBuilding.sqrMagnitude;

            if (dSqrToBuilding < closestDistanceSqr && dSqrToBuilding <= (targetingRange * targetingRange))
            {
                closestDistanceSqr = dSqrToBuilding;
                closestBuilding = building.transform;
            }
        }

        return closestBuilding;
    }

    private Transform FindClosestSwitcheableTowerWithinRange()
    {
        Transform closestSwitcheableTower = null;
        float closestDistanceSqr = Mathf.Infinity;

        foreach (GameObject tower in GameObject.FindGameObjectsWithTag("Tower"))
        {
            // We only consider towers that are switcheable
            Turret turretScript = tower.GetComponent<Turret>();
            if (turretScript.isSwitcheable) {
                Vector3 directionToSwitcheableTower = tower.transform.position - transform.position;
                float dSqrToSwitcheableTower = directionToSwitcheableTower.sqrMagnitude;

                if (dSqrToSwitcheableTower < closestDistanceSqr && dSqrToSwitcheableTower <= (targetingRange * targetingRange))
                {
                    closestDistanceSqr = dSqrToSwitcheableTower;
                    closestSwitcheableTower = tower.transform;
                }
            }
        }

        return closestSwitcheableTower;
    }
}
