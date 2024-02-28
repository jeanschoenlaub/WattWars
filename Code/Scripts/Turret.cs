using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEditor;
// using Unity.VisualScripting;
// using UnityEngine.UIElements;

public class Turret : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private Transform wireRotationPoint;
    [SerializeField] private GameObject wireObject;
    [SerializeField] private GameObject electronPrefab;
    [SerializeField] private Transform firingPoint;
    [SerializeField] private LayerMask enemyMask;

    [Header("Attribute")]
    [SerializeField] private float targetingRange = 5f;
    [SerializeField] private float electronPerSeconds = 1f;

    private Transform target;
    private float timeUntilFire;

    private void Update(){
        if (target == null){
            FindTarget();
            //If no target return
            ToggleWireVisibility(false);
            return;
        } else {
            float distance = Vector2.Distance(transform.position, target.position);
            if (distance >= targetingRange - 0.5f){
                target = null;
                ToggleWireVisibility(false);
                return; 
            }
        }

        RotateTowardsTarget();
        ScaleToTarget();

        int currentGameSpeed = LevelManager.GetGameSpeed();
        timeUntilFire += Time.deltaTime*currentGameSpeed;

        if (timeUntilFire >= 1f/ electronPerSeconds){
            Shoot(); 
            timeUntilFire = 0f;
        }

        ToggleWireVisibility(false);
    }

    private void Shoot(){
        GameObject electronObj = Instantiate(electronPrefab, firingPoint.position, Quaternion.identity);
        Electron electronScript = electronObj.GetComponent<Electron>();
        electronScript.SetTarget(target);
    }

    private void FindTarget()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, targetingRange, Vector2.zero, 0f, enemyMask);
        if (hits.Length > 0)
        {
            Transform furthestTarget = null;
            float maxProgress = -1f; // Start with a progress lower than any enemy could have

            foreach (RaycastHit2D hit in hits)
            {
                EnemyMovement enemyMovement = hit.transform.GetComponent<EnemyMovement>();

                if (enemyMovement != null && enemyMovement.pathProgress > maxProgress)
                {
                    furthestTarget = hit.transform;
                    maxProgress = enemyMovement.pathProgress;
                }
            }

            target = furthestTarget; // Set the found target
        }
    }

    //Only called if there is an actual target 
    private void RotateTowardsTarget(){
        float angle = Mathf.Atan2(target.position.y - transform.position.y,target.position.x - transform.position.x) * Mathf.Rad2Deg + 180f;
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0f,0f,angle));
        wireRotationPoint.rotation= targetRotation;
    }

    //Scale the wire lengths based on enemy position
    private void ScaleToTarget(){
        float distance = Vector2.Distance(transform.position, target.position);
        
        // Wire default scale is 32 bits half of a tile so we multiply by 2
        Vector3 newScale = wireObject.transform.localScale;
        newScale.x = distance / wireObject.transform.parent.localScale.x * 2; // Adjust for parent's scale if necessary
        wireObject.transform.localScale = newScale;
    }


    private void ToggleWireVisibility(bool isVisible){
        wireObject.SetActive(isVisible);
    }

    //Used to visualize range but can't seekm to keep for HTML5 export
    // private void OnDrawGizmosSelected(){
    //     Handles.color = Color.cyan;
    //     Handles.DrawWireDisc(transform.position, transform.forward, targetingRange);
    // }

    

}
