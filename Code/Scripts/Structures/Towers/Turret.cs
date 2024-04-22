using System.ComponentModel.Design;
using UnityEngine;

public enum TowerType
{
    Elec,
    Fuel,
    Gas,
    Coal,
    // Add more types as needed
}

public class Turret : MonoBehaviour
{
    [Header("Tower Configuration")]
    [SerializeField] private Tower towerConfig; // Reference to the Tower ScriptableObject

    [Header("--- All towers common attributes ")]
    [SerializeField] private GameObject bulletPrefab; //Either electron or fuel
    [SerializeField] private Transform firingPoint;
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private LayerMask buildingMask;
    [SerializeField] public TowerType towerType;
    
    [Header("--- Converting Tower Attribute ---")]
    [SerializeField] private GameObject switchOnOff;
    [SerializeField] public bool isSwitchable = false;
    [SerializeField] public ConversionAttribute conversion;
    [SerializeField] private Animator SwitchButtonAnim;

    private Transform furthestTarget;
    private float timeUntilFire;
    public bool isSwitchedOn = false; // Starts false but when we click down to build turns On

    private void Update(){
        // This is the shoot method only for ressource towers
        if (!isSwitchable ){
            
            FindTarget();

            if (furthestTarget){
                int currentGameSpeed = LevelManager.GetGameSpeed();
                timeUntilFire += Time.deltaTime*currentGameSpeed;

                if (timeUntilFire >= 1f/ towerConfig.bulletPerSeconds){
                    Shoot(); 
                    timeUntilFire = 0f;
                }
            }

            CheckIfInShade();
        }
    }

    private void Shoot(){
        GameObject bulletObj = Instantiate(bulletPrefab, firingPoint.position, Quaternion.identity);
        Bullet bulletScript = bulletObj.GetComponent<Bullet>();
        bulletScript.SetTarget(furthestTarget);
    }

    public void Convert(){
        FindTarget();
        Shoot(); 
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
            Transform furthestSwitcheableTarget = FindClosestSwitchableTowerWithinRange();
            
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
        // Use the enemy's scale to estimate its effective radius
        float enemyRadius = Mathf.Max(enemy.transform.localScale.x, enemy.transform.localScale.y, enemy.transform.localScale.z) * 0.5f;
        float adjustedRange = towerConfig.targetingRange + enemyRadius;
        bool isInRange = dSqrToTarget <= (adjustedRange * adjustedRange);

        if (enemyHealth == null || !isInRange)
        {
            return false; // Early exit if the enemy health component is missing or if the enemy is out of range
        }

        // Targeting logic based on tower's abilities
        if ( towerType == TowerType.Elec && enemyHealth.elecLives > 0)
        {
            // Tower can target enemies with elec lives
            return true;
        }
        else if (towerType == TowerType.Fuel && enemyHealth.fuelLives > 0)
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

        //TO-DO only target building of same types
        foreach (GameObject building in GameObject.FindGameObjectsWithTag("Building"))
        {
            Vector3 directionToBuilding = building.transform.position - transform.position;
            float dSqrToBuilding = directionToBuilding.sqrMagnitude;

            if (dSqrToBuilding < closestDistanceSqr && dSqrToBuilding <= (towerConfig.targetingRange * towerConfig.targetingRange))
            {
                closestDistanceSqr = dSqrToBuilding;
                closestBuilding = building.transform;
            }
        }

        return closestBuilding;
    }

    private Transform FindClosestSwitchableTowerWithinRange()
    {
        Transform closestSwitcheableTower = null;
        float closestDistanceSqr = Mathf.Infinity;

        foreach (GameObject tower in GameObject.FindGameObjectsWithTag("Tower"))
        {
            // We only consider towers that are switcheable
            Turret turretScript = tower.GetComponent<Turret>();
            // Check if the tower is switchable, switched on, and matches the required input type
            if (turretScript.isSwitchable && turretScript.isSwitchedOn && turretScript.conversion.inputType == towerType){
                Vector3 directionToSwitchableTower = tower.transform.position - transform.position;
                float dSqrToSwitcheableTower = directionToSwitchableTower.sqrMagnitude;

                if (dSqrToSwitcheableTower < closestDistanceSqr && dSqrToSwitcheableTower <= (towerConfig.targetingRange * towerConfig.targetingRange))
                {
                    closestDistanceSqr = dSqrToSwitcheableTower;
                    closestSwitcheableTower = tower.transform;
                }
            }
        }

        return closestSwitcheableTower;
    }

    private void CheckIfInShade(){
      
        bool isInShade = WeatherManager.main.CheckIfInTheShadeOfAnyActiveCloud(transform.position);

         SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();

        if (isInShade)
        {
            Debug.Log("In Shade");
            renderers[0].color = Color.red; // Corrected color reference
        }
        else
        {
            renderers[0].color = Color.white; // Corrected color reference
        }
    }
}
