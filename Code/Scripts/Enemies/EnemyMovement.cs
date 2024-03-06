using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] private float moveSpeed = 2f;

    private Transform target;
    private int pathIndex = 0;
    public float pathProgress = 0; // to track furthest along ennemy

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
                WaveManager.onEnemyDestroy.Invoke();
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
        pathProgress += step; //This is used by towers to find furthest ennemies
    }
}
