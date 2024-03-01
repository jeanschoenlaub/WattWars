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
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, targetingRange - 0.2f, Vector2.zero, 0f, enemyMask);
        if (hits.Length > 0)
        {
            Transform furthestTarget = null;
            int maxProgress = -1; // Start with a progress lower than any enemy could have

            foreach (RaycastHit2D hit in hits)
            {
                EnemyMovement enemyMovement = hit.transform.GetComponent<EnemyMovement>();
                Health enemyHealth = hit.transform.GetComponent<Health>();

                // First we check if ennemy still needs damage from our tower type (elec or fuel)
                // If yes we check if it's the furthest ennemy along, if yes --> target
                // HARDCODED ELEC FOR NOW
                if (enemyHealth == null || enemyHealth.elecLives != 0 ){
                    if (enemyMovement != null && enemyMovement.pathProgress > maxProgress  )
                    {
                        furthestTarget = hit.transform;
                        maxProgress = enemyMovement.pathProgress;
                    }
                }
            }
            target = furthestTarget; // Set the found target
        }

        // Then if we didn't find any ennemy to shoot we can target buildings
        if (target == null)
        {
            RaycastHit2D[] buildingHits = Physics2D.CircleCastAll(transform.position, targetingRange - 0.2f, Vector2.zero, 0f, buildingMask);
            if (buildingHits.Length > 0){
                target = buildingHits[0].transform;
            }
        }
    }
}
