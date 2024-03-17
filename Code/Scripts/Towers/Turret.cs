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
                if (enemyHealth != null && enemyHealth.elecLives != 0 && dSqrToTarget <= (targetingRange * targetingRange))
                {
                   if (enemyMovement.pathProgress > maxProgress)
                    {
                         maxProgress = enemyMovement.pathProgress;
                        furthestTarget = enemy.transform;
                       
                    }
                }
            }
        }

        // If no enemy is targeted, find the closest building
        if (furthestTarget == null)
        {
            closestDistance = Mathf.Infinity; // Reset for building search
            foreach (GameObject building in GameObject.FindGameObjectsWithTag("Building"))
            {
                Vector3 directionToBuilding = building.transform.position - transform.position;
                float dSqrToBuilding = directionToBuilding.sqrMagnitude;
                if (dSqrToBuilding < closestDistance && dSqrToBuilding <= (targetingRange * targetingRange))
                {
                    closestDistance = dSqrToBuilding;
                    furthestTarget = building.transform;
                }
            }
        }
    }
}
