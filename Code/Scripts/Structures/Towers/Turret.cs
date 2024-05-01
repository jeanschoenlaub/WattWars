using UnityEngine;

public enum TowerType
{
    Solar,
    NA,
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
    [SerializeField] public BulletType bulletType;
    [SerializeField] public TowerType towerType;
    
    [Header("--- Converting Tower Attribute ---")]
    [SerializeField] private GameObject switchOnOff;
    [SerializeField] public bool isSwitchable = false;
    [SerializeField] public ConversionAttribute conversion;
    [SerializeField] private Animator SwitchButtonAnim;
    private float currentCharge = 0f; // Starts false but when we click down to build turns On
    public bool isSwitchedOn = false; // Starts false but when we click down to build turns On

    // Internal variables
    private Transform furthestTarget;
    private float timeUntilFire;
   
    private void Update(){
        // This is the shoot method only for ressource towers
        int currentGameSpeed = LevelManager.GetGameSpeed();

        if (!isSwitchable){
            
            timeUntilFire += Time.deltaTime*currentGameSpeed;
            if (timeUntilFire >= 1f/ towerConfig.bulletPerSeconds){
                FindTarget();
                if (furthestTarget){
                    Shoot(); 
                    timeUntilFire = 0f;
                }
            }
        }

        // For battery towers
        else if (isSwitchable && isSwitchedOn) {
            if (currentCharge >= towerConfig.bulletDamage){
                FindTarget();
                if (furthestTarget){
                    timeUntilFire += Time.deltaTime*currentGameSpeed;

                    if (timeUntilFire >= 1f/ towerConfig.bulletPerSeconds){
                        Shoot(); ;
                        currentCharge = currentCharge -1;
                        UpdateBatterySprite();
                        timeUntilFire = 0f;
                    }
                }
            }
        }
    }

    private void Shoot(){

        float bulletDamage = towerConfig.bulletDamage;

        // But if Solar type we adjust elec damage depending on Weather
        if (towerType == TowerType.Solar){
            bulletDamage = adjustElecDamageSolar();
            if (bulletDamage == 0f){return;} // Don't shoot bullet visually if no damage, which can happen during night
        }

        GameObject bulletObj = Instantiate(bulletPrefab, firingPoint.position, Quaternion.identity);
        Bullet bulletScript = bulletObj.GetComponent<Bullet>();
        bulletScript.SetTarget(furthestTarget);
        bulletScript.SetBulletType(towerConfig.bulletType);
        bulletScript.SetDamage(bulletDamage);
    }

    public void Charge(float bulletDamage){
        if (currentCharge < towerConfig.maxCharge){
            currentCharge = currentCharge + bulletDamage;
            UpdateBatterySprite();
        }
    }

    public void UpdateBatterySprite(){
        Battery batteryScript = transform.GetComponent<Battery>();

        // Check if the Battery script was found
        if (batteryScript != null)
        {
            // Battery script found, set the sprite
            batteryScript.SetSprite(currentCharge, towerConfig.maxCharge);
        }
        else
        {
            // Battery script not found, log a warning message
            Debug.LogWarning("Battery script not found on GameObject: " + gameObject.name);
        }
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
        if (bulletType == BulletType.Elec && enemyHealth.elecLives > 0f)
        {
            // Tower can target enemies with elec lives
            return true;
        }
        else if (bulletType == BulletType.Fuel && enemyHealth.fuelLives > 0f)
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
        Transform currentTowerTransform = transform; // Store the current tower's transform

        foreach (GameObject towerObject in GameObject.FindGameObjectsWithTag("Tower"))
        {
            // Get the transform of the tower
            Transform towerTransform = towerObject.transform;

            // Skip the current tower (the one this script is attached to)
            if (towerTransform == currentTowerTransform)
            {
                continue;
            }
            
            // We only consider towers that are switcheable
            Turret turretScript = towerObject.GetComponent<Turret>();

            // Check if the tower is switchable, switched on, and matches the required input type
            if (turretScript.isSwitchable && turretScript.conversion.inputType == bulletType && turretScript.currentCharge < turretScript.towerConfig.maxCharge){
                Vector3 directionToSwitchableTower = towerTransform.position - transform.position;
                float dSqrToSwitcheableTower = directionToSwitchableTower.sqrMagnitude;

                if (dSqrToSwitcheableTower < closestDistanceSqr && dSqrToSwitcheableTower <= (towerConfig.targetingRange * towerConfig.targetingRange))
                {
                    closestDistanceSqr = dSqrToSwitcheableTower;
                    closestSwitcheableTower = towerTransform;
                }
            }
        }

        return closestSwitcheableTower;
    }


    private float adjustElecDamageSolar(){

        // If it is the night return 0 damage
        if (WeatherManager.main.isNight){
            return 0f;
        }

        // If is in shade of cloud return base damage*0.2
        bool isInShade = WeatherManager.main.CheckIfInTheShadeOfAnyActiveCloud(transform.position);
        if (isInShade)
        {
            return 0.2f*towerConfig.bulletDamage;
        }

        // If the night is falling 0.5x elec damage
        if (WeatherManager.main.isNightFalling){
            return 0.5f*towerConfig.bulletDamage;
        }

        else{return towerConfig.bulletDamage;}
    }
}
