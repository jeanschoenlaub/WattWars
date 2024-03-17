using UnityEngine;

public class Bullet : MonoBehaviour
{

    [Header("Attributes")]
    [SerializeField] private float bulletSpeed = 5f;
    [SerializeField] private int elecDamage = 1;
    [SerializeField] private int fuelDamage = 1;
    
    private Transform target;
    private float proximityDetectionRange = 0.1f; // If a bullet is closer than this to ennemy interacts

    public void SetTarget(Transform _target){
        target = _target; 
    }

    private void Update() {
        if (!target) {
            Destroy(gameObject); // Destroy the projectile if the target is null
            return;
        }

        MoveTowardsTarget();
        CheckProximityAndInteract();
    }

    private void CheckProximityAndInteract() {
        if (Vector2.Distance(transform.position, target.position) < proximityDetectionRange) {
            HandleInteractionBasedOnTag();
            Destroy(gameObject); // Destroy the projectile after interacting
        }
    }

    private void MoveTowardsTarget() {
        int currentGameSpeed = LevelManager.GetGameSpeed(); // Assuming you have a method to adjust game speed dynamically
        float step = bulletSpeed * currentGameSpeed * Time.deltaTime; // Calculate the step size
        transform.position = Vector2.MoveTowards(transform.position, target.position, step);
    }

    private void HandleInteractionBasedOnTag() {
        if (target.CompareTag("Enemy")) {
            DealDamageToEnemy();
        }
        else if (target.CompareTag("Building")) {
            TransferEnergyToBuilding();
        }
    }


    private void DealDamageToEnemy() {
        Health health = target.GetComponent<Health>();
        if (health != null) {
            health.TakeElecDamage(elecDamage);
            health.TakeFuelDamage(fuelDamage);
        }
    }


    private void TransferEnergyToBuilding() {
        GenerateMoney generateMoney = target.GetComponent<GenerateMoney>();
        if (generateMoney != null) {
            generateMoney.ReceiveEnergy(elecDamage); // Assuming 'amount' is defined elsewhere
        }
    }

}

