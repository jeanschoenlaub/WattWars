using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] private float moveSpeed = 2f;

    private Transform target;
    private int pathIndex = 0;
    public int pathProgress = 0; // to track furthest along ennemy

    private void Start(){
        target = LevelManager.main.path[pathIndex];
    }

    private void Update() {
        UpdatePathTarget();
        MoveTowardsTarget();
    }

   private void UpdatePathTarget() {
         if (Vector2.Distance(target.position, transform.position) <= 0.1f){
            pathIndex++;

            if (pathIndex == LevelManager.main.path.Length){
                EnemySpawner.onEnemyDestroy.Invoke();
                Destroy(gameObject);
                return;
            } else {
                target = LevelManager.main.path[pathIndex];
            }
        }
    }

    private void MoveTowardsTarget() {
        int currentGameSpeed = LevelManager.GetGameSpeed();
        float step = moveSpeed * currentGameSpeed * Time.deltaTime; // Calculate the step size
        transform.position = Vector2.MoveTowards(transform.position, target.position, step);

        // Adding logic to make the Sprite rotate based on direction but not really good
        // float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // float rotationOffset = 180f; // Becuase facing left initially
        // Quaternion rotation = Quaternion.AngleAxis(angle + rotationOffset, Vector3.forward);
        // rb.transform.rotation = rotation;// Apply the rotation to the transform
    }
}
