using UnityEngine;

public class Turret : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject electronPrefab;
    [SerializeField] private Transform firingPoint;
    [SerializeField] private GameObject switchOnOff;
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private LayerMask buildingMask;
    [SerializeField] private Animator SwitchButtonAnim;
    [SerializeField] private GameObject rangeCircle;

    [Header("Attribute")]
    [SerializeField] private float targetingRange = 5f;
    [SerializeField] private float electronPerSeconds = 1f;
    [SerializeField] private bool isSwitcheable = false;

    private Transform furthestTarget;
    private float timeUntilFire;
    private bool isSwitchedOn = false; // Starts false but when we click down to build turns On

    private void Update(){
 
        if (!isSwitcheable || (isSwitcheable && isSwitchedOn) ){
            
            FindTarget();

            if (furthestTarget){
                int currentGameSpeed = LevelManager.GetGameSpeed();
                timeUntilFire += Time.deltaTime*currentGameSpeed;

                if (timeUntilFire >= 1f/ electronPerSeconds){
                    Shoot(); 
                    timeUntilFire = 0f;
                }
            }

        }
       
    }

    private void Shoot(){
        GameObject electronObj = Instantiate(electronPrefab, firingPoint.position, Quaternion.identity);
        Bullet electronScript = electronObj.GetComponent<Bullet>();
        electronScript.SetTarget(furthestTarget);
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
            furthestTarget = FindClosestBuildingWithinRange();
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

    private bool IsEnemyTargetableAndInRange(GameObject enemy, out float enemyProgress)
    {
        enemyProgress = enemy.GetComponent<EnemyMovement>().pathProgress;
        Vector3 directionToTarget = enemy.transform.position - transform.position;
        float dSqrToTarget = directionToTarget.sqrMagnitude;
        Health enemyHealth = enemy.GetComponent<Health>();

        return enemyHealth != null && enemyHealth.elecLives != 0 && dSqrToTarget <= (targetingRange * targetingRange);
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
}
