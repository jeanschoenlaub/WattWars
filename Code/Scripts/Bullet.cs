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
        Debug.Log("Collision");
        
        // Check if the collided object is an enemy, then apply both types of damage
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy")){
            if (other.gameObject.GetComponent<Health>() != null){
                other.gameObject.GetComponent<Health>().TakeElecDamage(elecDamage);
                other.gameObject.GetComponent<Health>().TakeFuelDamage(fuelDamage);
            }
        }
        // Check if the collided object is a building, then receive enegry
        else if (other.gameObject.layer == LayerMask.NameToLayer("Buildings")){
            Debug.Log("hit building");
            if (other.gameObject.GetComponent<GenerateMoney>() != null){
                other.gameObject.GetComponent<GenerateMoney>().ReceiveEnergy(elecDamage); // Assuming 'amount' is defined elsewhere
            }
        }
    }
}

