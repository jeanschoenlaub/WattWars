using UnityEngine;

public class Bullet : MonoBehaviour
{
    
    [Header("References")]
    [SerializeField] private Rigidbody2D bullet;

    [Header("Attributes")]
    [SerializeField] private float bulletSpeed = 5f;
    [SerializeField] private int elecDamage = 1;
    [SerializeField] private int fuelDamage = 1;
    
    private Transform target;

    public void SetTarget(Transform _target){
        target = _target; 
    }

    private void FixedUpdate() {
        if (!target) {
            Destroy(gameObject); // Destroy the electron if the target is null
            return;
        }

        Vector2 direction = (target.position - transform.position).normalized;

        int currentGameSpeed = LevelManager.GetGameSpeed();
        bullet.velocity = direction * bulletSpeed * currentGameSpeed;
    }

    private void OnCollisionEnter2D(Collision2D other){
        Destroy(gameObject); //Destroys the electron
        other.gameObject.GetComponent<Health>().TakeElecDamage(elecDamage);
        other.gameObject.GetComponent<Health>().TakeFuelDamage(fuelDamage);
    }
}

